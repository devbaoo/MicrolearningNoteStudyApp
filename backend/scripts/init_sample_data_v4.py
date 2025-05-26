import boto3
from datetime import datetime, timedelta

def format_iso_date(date):
    """Format datetime for DynamoDB."""
    return date.strftime('%Y-%m-%dT%H:%M:%S.%f')[:-3] + 'Z'

def init_sample_data():
    """Initialize sample data for the Review System."""
    
    dynamodb = boto3.client('dynamodb', region_name='ap-southeast-1')
    
    # Current time
    current_time = datetime.utcnow()
    
    # Create a sample review session
    session = {
        'session_id': {'S': '00000000-0000-0000-0000-000000000021'},
        'timestamp': {'S': format_iso_date(current_time)}
    }
    
    try:
        dynamodb.put_item(
            TableName='ReviewSessions',
            Item=session
        )
        print(f"Added review session {session['session_id']['S']}")
    except Exception as e:
        print(f"Error adding review session: {e}")
    
    # Add review response
    response = {
        'response_id': {'S': '00000000-0000-0000-0000-000000000031'},
        'timestamp': {'S': format_iso_date(current_time)}
    }
    
    try:
        dynamodb.put_item(
            TableName='ReviewResponses',
            Item=response
        )
        print(f"Added review response {response['response_id']['S']}")
    except Exception as e:
        print(f"Error adding review response: {e}")
    
    print("Sample data initialization completed!")

if __name__ == '__main__':
    init_sample_data()
