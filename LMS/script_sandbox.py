import random
import csv
from random import randrange
from datetime import date, datetime, timedelta

def university_ID():
    ID = "u0"
    for c in range(0, 7):
        ID += str(random.randint(0, 9))
    return ID


def get_users():
    names = ()
    with open('users.csv', newline='') as csvfile:
        names = list(csv.reader(csvfile, delimiter=' '))
    return names


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


def dater():
    student_min_DOB = datetime.strptime('1/1/1990', '%m/%d/%Y').date()
    student_max_DOB = datetime.strptime('1/1/2000', '%m/%d/%Y').date()

    return date_generator(student_min_DOB, student_max_DOB)


def main():
    i = 0
    while i < 30:
        print(university_ID())
        print(dater())
        i += 1
    get_users()


if __name__ == '__main__':
    main()


