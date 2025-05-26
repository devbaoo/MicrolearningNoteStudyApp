import boto3
from datetime import datetime

def format_iso_date(date):
    """Format datetime for DynamoDB."""
    return date.strftime('%Y-%m-%dT%H:%M:%S.%f')[:-3] + 'Z'

def init_responses():
    """Initialize sample review responses."""
    
    dynamodb = boto3.client('dynamodb', region_name='ap-southeast-1')
    
    # Add review response
    response = {
        'response_id': {'S': '00000000-0000-0000-0000-000000000031'},
        'created_at': {'S': format_iso_date(datetime.utcnow())}
    }
    
    try:
        dynamodb.put_item(
            TableName='ReviewResponses',
            Item=response
        )
        print(f"Added review response {response['response_id']['S']}")
    except Exception as e:
        print(f"Error adding review response: {e}")

if __name__ == '__main__':
    init_responses()
