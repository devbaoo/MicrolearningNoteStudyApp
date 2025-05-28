import boto3
import json

def list_tables():
    # Initialize a DynamoDB client
    dynamodb = boto3.client('dynamodb')
    
    # List all tables
    try:
        response = dynamodb.list_tables()
        tables = response.get('TableNames', [])
        
        print("\nAvailable DynamoDB Tables:")
        print("-" * 80)
        
        if not tables:
            print("No tables found in the current region.")
            return
            
        # Get details for each table
        for table_name in tables:
            print(f"\nTable: {table_name}")
            print("-" * 40)
            
            try:
                # Get table description
                table_info = dynamodb.describe_table(TableName=table_name)
                table = table_info['Table']
                
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
                
                # Print global secondary indexes if any
                if 'GlobalSecondaryIndexes' in table:
                    print("\nGlobal Secondary Indexes:")
                    for gsi in table['GlobalSecondaryIndexes']:
                        print(f"  {gsi['IndexName']}:")
                        for key in gsi['KeySchema']:
                            print(f"    {key['AttributeName']} ({key['KeyType']})")
                
                # Print local secondary indexes if any
                if 'LocalSecondaryIndexes' in table:
                    print("\nLocal Secondary Indexes:")
                    for lsi in table['LocalSecondaryIndexes']:
                        print(f"  {lsi['IndexName']}:")
                        for key in lsi['KeySchema']:
                            print(f"    {key['AttributeName']} ({key['KeyType']})")
                
            except Exception as e:
                print(f"  Error getting table info: {str(e)}")
                
    except Exception as e:
        print(f"Error listing tables: {str(e)}")

if __name__ == "__main__":
    print("DynamoDB Table Information")
    print("=" * 80)
    list_tables()
