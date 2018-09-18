"""
Helper script for gathering real subjects and departments from the University of Utah's catelogue.

adam

"""

import csv
import datetime
from time import sleep
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from bs4 import BeautifulSoup


def get_driver():
    """Gets and returns the appropriate webdriver."""
    options = webdriver.ChromeOptions()
    options.add_argument('--headless')

    return webdriver.Chrome(
        executable_path=r'/usr/local/bin/chromedriver', chrome_options=options)


def connect_to_base(browser):
    """ Establishes the connection to the UofU course catelogue page.

        Timeout occurs after 3 attempts.

        Args:
            browser (browser): the browser driver (in this case chromedriver)
            page_number (int): an integer representing the requested page number

        Returns:
            False if connection fails 3 attempts, true if connection succesful.
    """

    base_url = f'https://student.apps.utah.edu/uofu/stu/ClassSchedules/main/1188/'
    connection_attempts = 0
    while connection_attempts < 3:
        try:
            browser.get(base_url)
            #WebDriverWait(browser, 5).until(EC.presence_of_element_located((By.ID, 'uu-main')))
            return True
        except Exception:
            connection_attempts += 1
            print(f'Error connecting to {base_url}.')
            print(f'Attempt #{connection_attempts}.')
    return False


def parse_html(html):
    """ Parses the html content of the department catelogue page.

        Scrapes and catelogues the following key metrics for each post.
            'subject' (string): the ID of the department.
            'department' (string): the full name of the department.

        Args:
            html ([string]): The source html of a target webpage.

        Returns:

    """
    soup = BeautifulSoup(html, 'html.parser')
    output_list = []
    class_list = soup.find_all('ul', class_='subject-list')

    for x in range(0, len(class_list)):

        subject_id = class_list[x].a.string
        department = class_list[x].span.string

        subject_department = {
            'subject_id': subject_id,
            'department': department,
        }
        output_list.append(subject_department)
    output_list = output_list[:-1]
    return output_list


def write_to_file(output_list, filename):
    """ Writes the results"""
    with open(filename, 'a') as csvfile:
        fieldnames = ['subject_id', 'department']
        writer = csv.DictWriter(csvfile, fieldnames=fieldnames)
        for row in output_list:
            writer.writerow(row)


def run_process(filename, browser):
    """ Runs the main scraping and results handilng process for each page."""
    if connect_to_base(browser):
        # follows robots.txt rules.
        sleep(2)
        html = browser.page_source
        output_list = parse_html(html)
        write_to_file(output_list, filename)
        print('Recorded {:d} departments.'.format(len(output_list)))
    else:
        print('Error connecting to course catelogue')


if __name__ == '__main__':
    """ Main method sets up script parameters and starts the script."""
    output_filename = f'departments.csv'
    browser = get_driver()
    with open(output_filename, 'a') as csvfile:
        fieldnames = ['subject_id', 'department']
        writer = csv.DictWriter(csvfile, fieldnames=fieldnames)

        print(f'Getting departments...')
        run_process(output_filename, browser)
        print(f'Department scraping complete!')
    browser.quit()