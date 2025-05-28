import boto3
import json

def check_notes_table():
    # Initialize a DynamoDB client
    dynamodb = boto3.client('dynamodb')
    
    try:
        # Get table description
        table_info = dynamodb.describe_table(TableName='Notes')
        table = table_info['Table']
        
        print("\nNotes Table Structure:")
        print("=" * 80)
        
        # Print basic info
        print(f"Status: {table['TableStatus']}")
        print(f"Item Count: {table.get('ItemCount', 'N/A')}")
        print(f"Creation Date: {table.get('CreationDateTime', 'N/A')}")
        
        # Print key schema
        print("\nKey Schema:")
        for key in table.get('KeySchema', []):
            print(f"  {key['AttributeName']} ({key['KeyType']})")
        
        # Print attribute definitions
        print("\nAttribute Definitions:")
        for attr in table.get('AttributeDefinitions', []):
            print(f"  {attr['AttributeName']} ({attr['AttributeType']})")
        
        # Print global secondary indexes
        if 'GlobalSecondaryIndexes' in table:
            print("\nGlobal Secondary Indexes:")
            for gsi in table['GlobalSecondaryIndexes']:
                print(f"  {gsi['IndexName']}:")
                for key in gsi['KeySchema']:
                    print(f"    {key['AttributeName']} ({key['KeyType']})")
        
        # Print local secondary indexes
        if 'LocalSecondaryIndexes' in table:
            print("\nLocal Secondary Indexes:")
            for lsi in table['LocalSecondaryIndexes']:
                print(f"  {lsi['IndexName']}:")
                for key in lsi['KeySchema']:
                    print(f"    {key['AttributeName']} ({key['KeyType']})")
        
        # Scan the table to see some sample items (first 5)
        print("\nSample Items (first 5):")
        response = dynamodb.scan(
            TableName='Notes',
            Limit=5,
            ReturnConsumedCapacity='TOTAL'
        )
        
        print(f"\nConsumed Capacity: {response.get('ConsumedCapacity', {}).get('CapacityUnits', 'N/A')} units")
        
        if 'Items' in response and response['Items']:
            for i, item in enumerate(response['Items'], 1):
                print(f"\nItem {i}:")
                print(json.dumps(item, indent=2, default=str))
        else:
            print("No items found in the table.")
        
    except Exception as e:
        print(f"Error: {str(e)}")
        if hasattr(e, 'response') and 'Error' in e.response:
            print(f"Error details: {e.response['Error']}")

if __name__ == "__main__":
    print("Checking Notes Table Structure and Sample Data")
    print("=" * 80)
    check_notes_table()
