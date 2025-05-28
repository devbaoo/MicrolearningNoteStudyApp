using Amazon.Lambda.Core;
using Common.Models;
using Common.Responses;
using System;
using System.Threading.Tasks;

namespace ReviewSystemFunction.Services
{
    public class SuperMemoService : ISuperMemoService
    {
        // SuperMemo-2 Algorithm Constants
        private const double DEFAULT_EASE_FACTOR = 2.5;
        private const double MIN_EASE_FACTOR = 1.3;
        private const double MAX_EASE_FACTOR = 3.0;
        private const double EASE_BONUS = 0.1;
        private const double EASE_PENALTY_MULTIPLIER = 0.08;

        // Performance thresholds
        private const double EXCELLENT_THRESHOLD = 0.9;
        private const double GOOD_THRESHOLD = 0.6;
        private const double POOR_THRESHOLD = 0.3;

        public Task<CalculateIntervalResponse> CalculateNextReviewIntervalAsync(
            ReviewAtom atomData,
            double successRating,
            int responseTimeMs,
            ILambdaContext context)
        {
            if (atomData == null)
            {
                throw new ArgumentNullException(nameof(atomData), "ReviewAtom cannot be null");
            }

            try
            {
                context.Logger.LogInformation($"Calculating interval for atom {atomData.Id}: success_rating={successRating}, response_time={responseTimeMs}ms");

                // Get current atom state with null checks
                var currentInterval = atomData.ReviewSchedule?.IntervalDays ?? 1;
                var easeFactor = atomData.ReviewSchedule?.EaseFactor ?? DEFAULT_EASE_FACTOR;
                var reviewCount = atomData.ReviewSchedule?.ReviewCount ?? 0;
                var difficultyScore = (double)(atomData.DifficultyScore ?? 0.5m);

                // Apply SuperMemo-2 algorithm with enhancements
                var (newInterval, newEaseFactor) = CalculateNewInterval(
                    currentInterval,
                    easeFactor,
                    reviewCount,
                    successRating,
                    responseTimeMs,
                    difficultyScore
                );

                // Calculate next review date
                var nextReviewDate = DateTime.UtcNow.AddDays(newInterval);

                // Determine performance category
                var performanceCategory = GetPerformanceCategory(successRating);

                // Calculate retention probability
                var retentionProbability = CalculateRetentionProbability(successRating, responseTimeMs, reviewCount);

                // Update difficulty score based on performance
                var newDifficultyScore = UpdateDifficultyScore(difficultyScore, successRating, responseTimeMs);

                context.Logger.LogInformation($"New interval calculated: {newInterval} days, ease factor: {newEaseFactor}");

                var response = new CalculateIntervalResponse
                {
                    AtomId = atomData.Id ?? throw new InvalidOperationException("AtomId cannot be null"),
                    NewIntervalDays = newInterval,
                    EaseFactor = newEaseFactor,
                    NextReviewDate = nextReviewDate,
                    PerformanceCategory = performanceCategory,
                    RetentionProbability = retentionProbability,
                    NewDifficultyScore = newDifficultyScore,
                    AlgorithmVersion = "SuperMemo-2-Enhanced-v1.0",
                    CalculationDetails = new IntervalCalculationDetails
                    {
                        PreviousInterval = currentInterval,
                        PreviousEaseFactor = easeFactor,
                        SuccessRating = successRating,
                        ResponseTimeMs = responseTimeMs,
                        ReviewCount = reviewCount + 1,
                        PerformanceFactors = new PerformanceFactors
                        {
                            SpeedBonus = CalculateSpeedBonus(responseTimeMs),
                            DifficultyAdjustment = CalculateDifficultyAdjustment(difficultyScore),
                            ConsistencyBonus = CalculateConsistencyBonus(reviewCount, successRating)
                        }
                    }
                };

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error in SuperMemo calculation: {ex.Message}");
                throw new InvalidOperationException($"Failed to calculate next review interval: {ex.Message}", ex);
            }
        }

        private (int newInterval, double newEaseFactor) CalculateNewInterval(
            int currentInterval,
            double easeFactor,
            int reviewCount,
            double successRating,
            int responseTimeMs,
            double difficultyScore)
        {
            var newInterval = currentInterval;
            var newEaseFactor = easeFactor;

            // SuperMemo-2 core algorithm
            if (successRating < POOR_THRESHOLD)
            {
                // Almost no recall - reset to beginning with penalty
                newInterval = 1;
                newEaseFactor = Math.Max(MIN_EASE_FACTOR, easeFactor - 0.2);
            }
            else if (successRating < GOOD_THRESHOLD)
            {
                // Partial recall - reduce interval slightly
                newInterval = Math.Max(1, (int)(currentInterval * 0.6));
                newEaseFactor = Math.Max(MIN_EASE_FACTOR, easeFactor - 0.15);
            }
            else
            {
                // Good or excellent recall - increase interval
                if (reviewCount == 0)
                {
                    newInterval = 1;
                }
                else if (reviewCount == 1)
                {
                    newInterval = 6;
                }
                else
                {
                    newInterval = (int)Math.Ceiling(currentInterval * easeFactor);
                }

                // Adjust ease factor based on performance quality
                var easeAdjustment = EASE_BONUS - ((5.0 - 5.0 * successRating) * EASE_PENALTY_MULTIPLIER);
                newEaseFactor = Math.Min(MAX_EASE_FACTOR, Math.Max(MIN_EASE_FACTOR, easeFactor + easeAdjustment));
            }

            // Apply enhancements
            newInterval = ApplyEnhancements(newInterval, successRating, responseTimeMs, difficultyScore, reviewCount);

            // Ensure minimum and maximum bounds
            newInterval = Math.Max(1, Math.Min(365, newInterval)); // Max 1 year interval

            return (newInterval, newEaseFactor);
        }

        private int ApplyEnhancements(int baseInterval, double successRating, int responseTimeMs, double difficultyScore, int reviewCount)
        {
            var adjustedInterval = (double)baseInterval;

            // Speed bonus: Faster responses get slight interval increase
            if (responseTimeMs > 0 && responseTimeMs < 3000) // Less than 3 seconds
            {
                var speedBonus = Math.Max(0, (3000 - responseTimeMs) / 30000.0); // Up to 10% bonus
                adjustedInterval *= (1.0 + speedBonus);
            }

            // Difficulty adjustment: Easier atoms get longer intervals
            var difficultyMultiplier = 2.0 - difficultyScore; // Range: 1.0 to 2.0
            adjustedInterval *= Math.Pow(difficultyMultiplier, 0.1); // Gentle adjustment

            // Consistency bonus: Multiple successful reviews get bonus
            if (successRating >= GOOD_THRESHOLD && reviewCount >= 3)
            {
                var consistencyBonus = Math.Min(0.2, reviewCount * 0.02); // Up to 20% bonus
                adjustedInterval *= (1.0 + consistencyBonus);
            }


            // Excellence bonus: Perfect or near-perfect recall
            if (successRating >= EXCELLENT_THRESHOLD)
            {
                adjustedInterval *= 1.1; // 10% bonus for excellence
            }

            return (int)Math.Ceiling(adjustedInterval);
        }

        private string GetPerformanceCategory(double successRating)
        {
            return successRating switch
            {
                >= EXCELLENT_THRESHOLD => "Excellent",
                >= GOOD_THRESHOLD => "Good",
                >= POOR_THRESHOLD => "Fair",
                _ => "Needs Review"
            };
        }

        private double CalculateRetentionProbability(double successRating, int responseTimeMs, int reviewCount)
        {
            // Base retention from success rating
            var baseRetention = successRating;

            // Adjust for response time (faster = more confident = higher retention)
            var timeAdjustment = responseTimeMs > 0 ? Math.Max(0, (10000 - responseTimeMs) / 10000.0 * 0.1) : 0;

            // Adjust for review history (more reviews = more stable retention)
            var historyAdjustment = Math.Min(0.1, reviewCount * 0.01);

            return Math.Min(1.0, baseRetention + timeAdjustment + historyAdjustment);
        }

        private double UpdateDifficultyScore(double currentDifficulty, double successRating, int responseTimeMs)
        {
            // Adjust difficulty based on performance
            var difficultyAdjustment = 0.0;

            // Success rating impact (good performance = easier)
            difficultyAdjustment -= (successRating - 0.5) * 0.1;

            // Response time impact (fast response = easier)
            if (responseTimeMs > 0)
            {
                var normalizedTime = Math.Min(1.0, responseTimeMs / 10000.0); // Normalize to 0-1
                difficultyAdjustment += (normalizedTime - 0.5) * 0.05;
            }

            var newDifficulty = currentDifficulty + difficultyAdjustment;
            return Math.Max(0.1, Math.Min(1.0, newDifficulty));
        }

        private double CalculateSpeedBonus(int responseTimeMs)
        {
            if (responseTimeMs <= 0) return 0;
            return Math.Max(0, (5000 - responseTimeMs) / 50000.0); // Up to 10% bonus for sub-5-second responses
        }

        private double CalculateDifficultyAdjustment(double difficultyScore)
        {
            return (1.0 - difficultyScore) * 0.1; // Easier atoms get slight bonus
        }

        private double CalculateConsistencyBonus(int reviewCount, double successRating)
        {
            if (reviewCount < 2 || successRating < GOOD_THRESHOLD) return 0;
            return Math.Min(0.15, reviewCount * 0.01); // Up to 15% bonus for consistent performance
        }
    }
}
