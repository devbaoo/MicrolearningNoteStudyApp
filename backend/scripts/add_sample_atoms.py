import boto3
import random
import uuid
from datetime import datetime, timedelta
from typing import List, Dict, Any
import json
import os

# Configure AWS credentials if not already configured
os.environ['AWS_DEFAULT_REGION'] = 'ap-southeast-1'  # Update with your region if different

def format_iso_date(date: datetime) -> str:
    """Format datetime for DynamoDB."""
    return date.strftime('%Y-%m-%dT%H:%M:%S.%f')[:-3] + 'Z'

def create_sample_atoms(user_id: str, count: int) -> List[Dict[str, Any]]:
    """Generate sample atom data."""
    atom_types = ["concept", "fact", "principle", "process"]
    subjects = ["Math", "Science", "History", "Programming", "Language", "Art"]
    
    atoms = []
    current_time = datetime.utcnow().replace(microsecond=0)  # Remove microseconds for cleaner timestamps
    current_time_str = current_time.strftime('%Y-%m-%dT%H:%M:%S.000Z')
    
    for i in range(1, count + 1):
        atom_type = random.choice(atom_types)
        subject = random.choice(subjects)
        difficulty = round(random.uniform(0.1, 0.9), 2)
        importance = round(random.uniform(0.3, 1.0), 2)
        
        # Create review intervals based on difficulty
        days_due = int((1 - difficulty) * 30) + 1  # 1-30 days
        next_review = current_time + timedelta(days=days_due)
        
        # Create base atom with all required fields for DynamoDB
        atom_id = str(uuid.uuid4())
        created_at = current_time_str
        next_review_str = next_review.strftime('%Y-%m-%dT%H:%M:%S.000Z')
        
        # Create a sample note ID for reference
        note_id = f'note-{i:04d}'
        
        # Create tags list with subject and type
        tags = [subject, atom_type, f'generated-{current_time.strftime("%Y%m%d")}']
        
        atom = {
            'atom_id': {'S': atom_id},
            'user_id': {'S': user_id},
            'content': {'S': f'Sample {atom_type.capitalize()} about {subject} {i}'},
            'type': {'S': atom_type},
            'importance_score': {'N': str(importance)},
            'difficulty_score': {'N': str(difficulty)},
            'current_interval': {'N': str(days_due)},
            'ease_factor': {'N': '2.5'},
            'review_count': {'N': '0'},
            'next_review_date': {'S': next_review_str},
            'last_review_date': {'S': created_at},  # Same as created_at for new atoms
            'created_at': {'S': created_at},
            'updated_at': {'S': created_at},
            'note_id': {'S': note_id},
            'tags': {'SS': tags}
        }
        
        # Add some atoms with past due dates
        if i % 5 == 0:
            past_due = current_time - timedelta(days=random.randint(1, 14))
            atom['NextReviewDate'] = {'S': format_iso_date(past_due)}
        
        # Add some atoms with review history
        if i % 3 == 0:
            atom['ReviewCount'] = {'N': str(random.randint(1, 10))}
            last_review = current_time - timedelta(days=random.randint(1, 30))
            atom['LastReviewDate'] = {'S': format_iso_date(last_review)}
        
        atoms.append(atom)
    
    return atoms

def add_sample_atoms():
    """Add sample atoms to DynamoDB."""
    # Configuration
    TABLE_NAME = 'Atoms'  # Table name from ReviewService
    USER_ID = 'sample-user-1'  # Replace with actual user ID
    NUMBER_OF_ATOMS = 10  # Reduced number of atoms for testing
    
    # Configure AWS credentials if not already set
    session = boto3.Session(
        aws_access_key_id=os.getenv('AWS_ACCESS_KEY_ID'),
        aws_secret_access_key=os.getenv('AWS_SECRET_ACCESS_KEY'),
        region_name=os.getenv('AWS_DEFAULT_REGION', 'ap-southeast-1')
    )
    
    # Initialize DynamoDB client with the configured session
    dynamodb = session.client('dynamodb')
    
    # Initialize DynamoDB client is now done above with session
    
    # Generate sample atoms
    print(f"Generating {NUMBER_OF_ATOMS} sample atoms...")
    atoms = create_sample_atoms(USER_ID, NUMBER_OF_ATOMS)
    
    # Add atoms to DynamoDB
    print(f"Adding {len(atoms)} atoms to DynamoDB table '{TABLE_NAME}'...")
    
    success_count = 0
    for atom in atoms:
        try:
            dynamodb.put_item(
                TableName=TABLE_NAME,
                Item=atom
            )
            success_count += 1
            if success_count % 10 == 0:
                print(f"Added {success_count} atoms so far...")
        except Exception as e:
            print(f"Error adding atom {atom.get('atom_id', {}).get('S', 'unknown')}: {e}")
            print(f"Error details: {str(e)}")
            # Print the full atom data for debugging
            print("Atom data that caused the error:")
            print(json.dumps(atom, indent=2))
    
    print(f"\nSuccessfully added {success_count} out of {len(atoms)} atoms to the {TABLE_NAME} table.")

if __name__ == '__main__':
    add_sample_atoms()
