import boto3
import random
import uuid
from datetime import datetime, timedelta
from typing import List, Dict, Any
import os
import json
from faker import Faker

# Configure AWS credentials if not already configured
os.environ['AWS_DEFAULT_REGION'] = 'ap-southeast-1'  # Update with your region if different

# Initialize Faker for realistic test data
fake = Faker()

def format_iso_date(date: datetime) -> str:
    """Format datetime for DynamoDB."""
    return date.strftime('%Y-%m-%dT%H:%M:%S.000Z')

def create_sample_users(count: int) -> List[Dict[str, Any]]:
    """Generate sample user data."""
    users = []
    
    for i in range(1, count + 1):
        user_id = f'user-{i:03d}'
        username = fake.user_name()
        email = f"{username}@example.com"
        
        user = {
            'UserId': {'S': user_id},
            'Email': {'S': email},
            'Username': {'S': username},
            'CreatedAt': {'S': format_iso_date(datetime.utcnow())},
            'IsActive': {'BOOL': True},
            'Image': {'S': f"https://i.pravatar.cc/150?u={user_id}"}
        }
        users.append(user)
    
    return users

def create_sample_notes(users: List[Dict[str, Any]], count_per_user: int) -> List[Dict[str, Any]]:
    """Generate sample note data."""
    notes = []
    formats = ["cornell", "zettelkasten", "mindmap", "plain"]
    sources = ["manual", "imported", "web", "pdf"]
    
    for user in users:
        try:
            user_id = user['UserId']['S']
            log_message(f"Creating notes for user {user_id}")
            
            for i in range(count_per_user):
                try:
                    note_id = f'note-{user_id}-{i:03d}'
                    title = fake.sentence(nb_words=4)
                    content = '\n\n'.join([fake.paragraph(nb_sentences=5) for _ in range(3)])
                    
                    # Ensure we don't have empty tags
                    tags = [fake.word() for _ in range(random.randint(1, 4))]
                    if not all(tags):
                        tags = ["sample"]
                    
                    # Get current timestamp as Unix timestamp (number)
                    current_time = int(datetime.utcnow().timestamp())
                    
                    # Create note with proper attribute types
                    note = {
                        'NoteId': {'S': note_id},
                        'UserId': {'S': user_id},
                        'Title': {'S': title},
                        'Content': {'S': content},
                        'Format': {'S': random.choice(formats)},
                        'Tags': {'SS': tags},
                        'CreatedAt': {'N': str(current_time)},  # Changed to N for number type
                        'UpdatedAt': {'N': str(current_time)},   # Changed to N for number type
                        'IsArchived': {'BOOL': False},
                        'SourceType': {'S': random.choice(sources)},
                        'SourceUrl': {'S': f"https://example.com/notes/{note_id}" if random.random() > 0.3 else ''},
                        'QualityScore': {'N': str(round(random.uniform(0.5, 1.0), 2))},
                        'KnowledgeDensity': {'N': str(round(random.uniform(0.1, 0.9), 2))},
                        'WordCount': {'N': str(random.randint(50, 500))},
                        'AtomCount': {'N': str(random.randint(1, 10))}
                    }
                    notes.append(note)
                    log_message(f"  Created note {i+1}/{count_per_user} for user {user_id}")
                except Exception as e:
                    log_message(f"  Error creating note {i+1} for user {user_id}: {str(e)}")
                    import traceback
                    log_message(f"  {traceback.format_exc()}")
        except Exception as e:
            log_message(f"Error processing user: {str(e)}")
            import traceback
            log_message(f"{traceback.format_exc()}")
    
    return notes

def create_sample_atoms(users: List[Dict[str, Any]], notes: List[Dict[str, Any]], count_per_user: int) -> List[Dict[str, Any]]:
    """Generate sample atom data."""
    atoms = []
    atom_types = ["concept", "fact", "principle", "process"]
    subjects = ["Math", "Science", "History", "Programming", "Language", "Art"]
    
    current_time = datetime.utcnow()
    current_time_str = format_iso_date(current_time)
    
    for user in users:
        user_id = user['UserId']['S']
        user_notes = [n for n in notes if n['UserId']['S'] == user_id]
        
        if not user_notes:
            continue
            
        for i in range(count_per_user):
            atom_type = random.choice(atom_types)
            subject = random.choice(subjects)
            difficulty = round(random.uniform(0.1, 0.9), 2)
            importance = round(random.uniform(0.3, 1.0), 2)
            days_due = int((1 - difficulty) * 30) + 1
            next_review = current_time + timedelta(days=days_due)
            next_review_str = format_iso_date(next_review)
            
            # Select a random note from this user's notes
            note = random.choice(user_notes)
            
            atom = {
                'atom_id': {'S': str(uuid.uuid4())},
                'user_id': {'S': user_id},
                'content': {'S': f'Sample {atom_type.capitalize()} about {subject} {i+1} for {user_id}'},
                'type': {'S': atom_type},
                'importance_score': {'N': str(importance)},
                'difficulty_score': {'N': str(difficulty)},
                'current_interval': {'N': str(days_due)},
                'ease_factor': {'N': '2.5'},
                'review_count': {'N': '0'},
                'next_review_date': {'S': next_review_str},
                'last_review_date': {'S': current_time_str},
                'created_at': {'S': current_time_str},
                'updated_at': {'S': current_time_str},
                'note_id': {'S': note['NoteId']['S']},
                'tags': {'SS': [subject, atom_type, f'generated-{current_time.strftime("%Y%m%d")}']}
            }
            
            # Make some atoms past due
            if i % 3 == 0:
                past_due = current_time - timedelta(days=random.randint(1, 14))
                atom['next_review_date'] = {'S': format_iso_date(past_due)}
            
            # Add some review history
            if i % 4 == 0:
                atom['review_count'] = {'N': str(random.randint(1, 10))}
                last_review = current_time - timedelta(days=random.randint(1, 30))
                atom['last_review_date'] = {'S': format_iso_date(last_review)}
            
            atoms.append(atom)
    
    return atoms

def list_tables(dynamodb):
    """List all available DynamoDB tables."""
    try:
        response = dynamodb.list_tables()
        return response.get('TableNames', [])
    except Exception as e:
        print(f"Error listing tables: {str(e)}")
        return []

def verify_table_exists(dynamodb, table_name):
    """Verify if a table exists and is active."""
    try:
        response = dynamodb.describe_table(TableName=table_name)
        return response['Table']['TableStatus'] == 'ACTIVE'
    except dynamodb.exceptions.ResourceNotFoundException:
        return False
    except Exception as e:
        print(f"Error verifying table {table_name}: {str(e)}")
        return False

def log_message(message):
    """Helper function to log messages with timestamps."""
    timestamp = datetime.utcnow().strftime('%Y-%m-%d %H:%M:%S')
    print(f"[{timestamp}] {message}")

def add_sample_data():
    """Add sample data to DynamoDB."""
    # Configuration - these should match your actual table names in DynamoDB
    TABLE_NAMES = {
        'Users': 'User',  # Note the singular 'User' based on your table name
        'Notes': 'Notes',
        'Atoms': 'Atoms'
    }
    
    NUM_USERS = 20
    NOTES_PER_USER = 1  # 20 users * 1 note each = 20 notes
    ATOMS_PER_USER = 1  # 20 users * 1 atom each = 20 atoms
    
    # Configure AWS session
    session = boto3.Session(
        aws_access_key_id=os.getenv('AWS_ACCESS_KEY_ID'),
        aws_secret_access_key=os.getenv('AWS_SECRET_ACCESS_KEY'),
        region_name=os.getenv('AWS_DEFAULT_REGION', 'ap-southeast-1')
    )
    
    # Initialize DynamoDB client
    dynamodb = session.client('dynamodb')
    
    # List all available tables
    log_message("Checking available tables in DynamoDB...")
    available_tables = list_tables(dynamodb)
    log_message(f"Found {len(available_tables)} tables:")
    for table in available_tables:
        log_message(f"- {table}")
    
    # Verify required tables exist
    missing_tables = []
    for table_type, table_name in TABLE_NAMES.items():
        if table_name not in available_tables:
            missing_tables.append(table_name)
    
    if missing_tables:
        log_message(f"Error: The following tables are missing: {', '.join(missing_tables)}")
        log_message("Please create these tables in DynamoDB first.")
        return
    
    # Verify each table is active
    log_message("Verifying table status...")
    for table_type, table_name in TABLE_NAMES.items():
        if not verify_table_exists(dynamodb, table_name):
            log_message(f"Error: Table {table_name} exists but is not active")
            return
        log_message(f"- {table_name} is active")
    
    try:
        # Create sample data
        log_message(f"Generating {NUM_USERS} users...")
        users = create_sample_users(NUM_USERS)
        log_message(f"Generated {len(users)} users")
        
        log_message(f"Generating {NUM_USERS * NOTES_PER_USER} notes...")
        notes = create_sample_notes(users, NOTES_PER_USER)
        log_message(f"Generated {len(notes)} notes")
        
        log_message(f"Generating {NUM_USERS * ATOMS_PER_USER} atoms...")
        atoms = create_sample_atoms(users, notes, ATOMS_PER_USER)
        log_message(f"Generated {len(atoms)} atoms")
        
        # Add users to DynamoDB
        log_message(f"Adding {len(users)} users to DynamoDB table '{TABLE_NAMES['Users']}'...")
        user_count = 0
        for i, user in enumerate(users, 1):
            try:
                user_id = user.get('UserId', {}).get('S', f'user-{i}')
                log_message(f"Adding user {i}/{len(users)}: {user_id}")
                response = dynamodb.put_item(TableName=TABLE_NAMES['Users'], Item=user, ReturnConsumedCapacity='TOTAL')
                log_message(f"  Consumed capacity: {response.get('ConsumedCapacity', {}).get('CapacityUnits', 'N/A')} units")
                user_count += 1
            except Exception as e:
                log_message(f"Error adding user {user_id}: {str(e)}")
        
        # Add notes to DynamoDB
        log_message(f"Adding {len(notes)} notes to DynamoDB table '{TABLE_NAMES['Notes']}'...")
        note_count = 0
        for i, note in enumerate(notes, 1):
            try:
                note_id = note.get('NoteId', {}).get('S', f'note-{i}')
                log_message(f"Adding note {i}/{len(notes)}: {note_id}")
                
                # Log the note data being sent to DynamoDB
                log_message(f"  Note data: {json.dumps(note, default=str)}")
                
                # Add the note to DynamoDB
                response = dynamodb.put_item(
                    TableName=TABLE_NAMES['Notes'],
                    Item=note,
                    ReturnConsumedCapacity='TOTAL',
                    ReturnItemCollectionMetrics='SIZE',
                    ReturnValues='NONE'
                )
                
                log_message(f"  Successfully added note {note_id}")
                log_message(f"  Consumed capacity: {response.get('ConsumedCapacity', {}).get('CapacityUnits', 'N/A')} units")
                note_count += 1
                
            except dynamodb.exceptions.ConditionalCheckFailedException as e:
                log_message(f"  Conditional check failed for note {note_id}: {str(e)}")
            except dynamodb.exceptions.ProvisionedThroughputExceededException as e:
                log_message(f"  Provisioned throughput exceeded for note {note_id}: {str(e)}")
                time.sleep(1)  # Add a small delay
            except dynamodb.exceptions.ResourceNotFoundException as e:
                log_message(f"  Table {TABLE_NAMES['Notes']} not found: {str(e)}")
                break  # No point continuing if table doesn't exist
            except Exception as e:
                log_message(f"  Error adding note {note_id}: {str(e)}")
                import traceback
                log_message(f"  {traceback.format_exc()}")
                
                # Try to get more details about the error
                if hasattr(e, 'response') and 'Error' in e.response:
                    log_message(f"  Error details: {e.response['Error']}")
                    if 'message' in e.response['Error']:
                        log_message(f"  Message: {e.response['Error']['message']}")
                    if 'Code' in e.response['Error']:
                        log_message(f"  Error code: {e.response['Error']['Code']}")
        
        # Add atoms to DynamoDB
        log_message(f"Adding {len(atoms)} atoms to DynamoDB table '{TABLE_NAMES['Atoms']}'...")
        atom_count = 0
        for i, atom in enumerate(atoms, 1):
            try:
                atom_id = atom.get('atom_id', {}).get('S', f'atom-{i}')
                log_message(f"Adding atom {i}/{len(atoms)}: {atom_id}")
                response = dynamodb.put_item(TableName=TABLE_NAMES['Atoms'], Item=atom, ReturnConsumedCapacity='TOTAL')
                log_message(f"  Consumed capacity: {response.get('ConsumedCapacity', {}).get('CapacityUnits', 'N/A')} units")
                atom_count += 1
            except Exception as e:
                log_message(f"Error adding atom {atom_id}: {str(e)}")
        
        log_message("\nSample data generation completed!")
        log_message(f"- Successfully added {user_count} out of {len(users)} users")
        log_message(f"- Successfully added {note_count} out of {len(notes)} notes")
        log_message(f"- Successfully added {atom_count} out of {len(atoms)} atoms")
        
        if note_count < len(notes) or atom_count < len(atoms):
            log_message("\nSome items failed to be added. Please check the error messages above.")
        
    except Exception as e:
        log_message(f"\nError during data generation: {str(e)}")
        import traceback
        traceback.print_exc()

if __name__ == '__main__':
    log_message("Starting sample data generation...")
    try:
        add_sample_data()
    except Exception as e:
        log_message(f"Fatal error: {str(e)}")
        import traceback
        traceback.print_exc()
    log_message("Script execution completed.")
