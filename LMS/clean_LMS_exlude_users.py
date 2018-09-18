import mysql.connector
from mysql.connector import errorcode

def clear_db(cursor, cnx):
    tables = ('Administrators', 'Assignment', 'AssignmentCat', 'Class', 'Course',
              'Department', 'Submissions')

    truncate = 'DELETE FROM '

    for t in tables:
        print(truncate + t)
        cursor.execute(truncate + t)
    cnx.commit()

def main():

    config = {
        'user': 'u0432850',
        'password': 'jelly',
        'host': 'atr.eng.utah.edu',
        'database': 'Team6',
        'raise_on_warnings': True,
        'use_pure': False,
    }

    try:
        cnx = mysql.connector.connect(**config)
    except mysql.connector.Error as err:
        if err.errno == errorcode.ER_ACCESS_DENIED_ERROR:
            print("Something is wrong with your user name or password")
        elif err.errno == errorcode.ER_BAD_DB_ERROR:
            print("Database does not exist")
        else:
            print(err)

    print('Connected to ' + config['host'])
    cursor = cnx.cursor()

    clear_db(cursor, cnx)

    cnx.close()
    print('Cleared ' + config['host'] + ' ' + config['database'])


if __name__ == '__main__':
    main()