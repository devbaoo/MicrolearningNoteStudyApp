import boto3

def verify_notes():
    # Initialize a DynamoDB client
    dynamodb = boto3.client('dynamodb')
    
    try:
        # First, get the count of items in the table
        count_response = dynamodb.scan(
            TableName='Notes',
            Select='COUNT'
        )
        
        total_items = count_response.get('Count', 0)
        print(f"Total items in Notes table: {total_items}")
        
        # Now get a few sample items
        if total_items > 0:
            print("\nSample items from Notes table:")
            print("=" * 80)
            
            scan_response = dynamodb.scan(
                TableName='Notes',
                Limit=min(5, total_items)  # Get up to 5 items
            )
            
            for i, item in enumerate(scan_response.get('Items', []), 1):
                print(f"\nItem {i}:")
                # Print a simplified view of the item
                print(f"  NoteId: {item.get('NoteId', {}).get('S', 'N/A')}")
                print(f"  UserId: {item.get('UserId', {}).get('S', 'N/A')}")
                print(f"  Title: {item.get('Title', {}).get('S', 'N/A')}")
                print(f"  CreatedAt: {item.get('CreatedAt', {}).get('N', 'N/A')}")
                print(f"  Format: {item.get('Format', {}).get('S', 'N/A')}")
                print(f"  Tags: {', '.join(item.get('Tags', {}).get('SS', ['N/A']))}")
        
        # Check if our sample data was added by looking for our pattern
        print("\nChecking for sample data:")
        print("=" * 80)
        
        # Try to find a note with our pattern (note-user-XXX-000)
        scan_response = dynamodb.scan(
            TableName='Notes',
            FilterExpression='begins_with(NoteId, :prefix)',
            ExpressionAttributeValues={
                ':prefix': {'S': 'note-user-'}
            },
            Limit=3
        )
        
        sample_notes = scan_response.get('Items', [])
        if sample_notes:
            print("\nFound sample notes:")
            for i, note in enumerate(sample_notes, 1):
                print(f"  {i}. {note.get('NoteId', {}).get('S', 'N/A')} - {note.get('Title', {}).get('S', 'No title')}")
        else:
            print("No sample notes found with the expected pattern.")
        
    except Exception as e:
        print(f"Error: {str(e)}")
        if hasattr(e, 'response') and 'Error' in e.response:
            print(f"Error details: {e.response['Error']}")

if __name__ == "__main__":
    print("Verifying Notes in DynamoDB")
    print("=" * 80)
    verify_notes()
