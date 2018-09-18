from __future__ import print_function
from datetime import date, datetime, timedelta
import mysql.connector
from mysql.connector import errorcode
import csv
from random import randrange
import random


def get_departments():
    departments = ()
    with open('departments_short.csv', newline='') as csvfile:
        departments = list(csv.reader(csvfile, delimiter=','))

    return departments


def get_users():
    names = ()
    with open('users.csv', newline='') as csvfile:
        names = list(csv.reader(csvfile, delimiter=' '))

    return names


def get_courses():
    courses = ()
    with open('courses.csv', newline='') as csvfile:
        courses = list(csv.reader(csvfile, delimiter='\t'))

    return courses

departments = get_departments()
names = get_users()
courses = get_courses()


def date_generator(start, end):
    """
    Helper function for testing, will return a random datetime between two datetime 
    objects.

        Args:
            start (datetime) : minimum boundary for datetime generator
            page_number (datetime) : maximum boundary for datetime generator

        Returns:
            date (datetime) : a random datetime between start and end
    """
    delta = end - start
    int_delta = (delta.days * 24 * 60 * 60) + delta.seconds
    random_second = randrange(int_delta)

    return start + timedelta(seconds=random_second)


def universityID_generator():
    ID = "u0"
    for c in range(0, 6):
        ID += str(random.randint(0, 9))

    return ID


def add_departments(cursor, cnx):

    add_department = ("INSERT INTO `Department`"
                      " (`Subject`, `Name`)"
                      "VALUES (%s, %s)")

    for r in range(0, len(departments)):
        department_data = (departments[r][0], departments[r][1])
        print(add_department, department_data)
        cursor.execute(add_department, department_data)
 
    cnx.commit()


def add_courses(cursor, cnx):
    add_course = ("INSERT INTO `Course`"
                  " (`Department`, `Number`, `Name`)"
                  "VALUES (%s, %s, %s)")

    for r in range(0, len(courses)):
        course_data = (courses[r][0], courses[r][1], courses[r][2])
        print(add_course, course_data)
        cursor.execute(add_course, course_data)

    cnx.commit()


def add_users(cursor, cnx):

    courses = ('CS', 'ART', 'BIOEN', 'JAPAN', 'MSE')
    num_profs = len(names) % 50
    professors = names[:num_profs]
    students = names[num_profs:]

    student_min_DOB = datetime.strptime('1/1/1990', '%m/%d/%Y').date()
    student_max_DOB = datetime.strptime('1/1/2000', '%m/%d/%Y').date()

    prof_min_DOB = datetime.strptime('1/1/1960', '%m/%d/%Y').date()
    prof_max_DOB = datetime.strptime('1/1/1990', '%m/%d/%Y').date()

    add_user = ("INSERT INTO `Users`"
                " (`uID`, `First`, `Last`, `DOB`, `Password`)"
                "VALUES (%s, %s, %s, %s, %s)")

    add_professor = ("INSERT INTO `Professors`"
                     " (`uID`, `Department`)"
                     "VALUES (%s, %s)")

    add_student = ("INSERT INTO `Students`"
                   " (`uID`, `Major`)"
                   "VALUES (%s, %s)")

    # add professors
    for r in range(0, len(professors)):
        university_ID = universityID_generator()
        DOB = date_generator(prof_min_DOB, prof_max_DOB).date()
        professor_data = (university_ID, courses[r % 5])
        user_data = (
            university_ID, professors[r][0], professors[r][1], DOB, 'j')

        print(add_user, user_data)
        print(add_professor, professor_data)
        cursor.execute(add_user, user_data)
        cursor.execute(add_professor, professor_data)

    cnx.commit()

    # add students
    for r in range(0, len(students)):
        university_ID = universityID_generator()
        DOB = date_generator(student_min_DOB, student_max_DOB)
        student_data = (university_ID, courses[r % 5])
        user_data = (
            university_ID, students[r][0], students[r][1], DOB, 'j')

        print(add_user, user_data)
        print(add_student, student_data)
        cursor.execute(add_user, user_data)
        cursor.execute(add_student, student_data)

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
    add_departments(cursor, cnx)
    add_courses(cursor, cnx)

    cnx.close()


if __name__ == '__main__':
    main()
