import requests, csv
from bs4 import BeautifulSoup


# https://stackoverflow.com/questions/23377533/python-beautifulsoup-parsing-table
def read_country_codes():
    url = 'https://www.iban.com/country-codes'
    html = requests.get(url).text
    soup = BeautifulSoup(html, 'html.parser')
    data = []
    table = soup.find('table', attrs={'id': 'myTable'})
    table_body = table.find('tbody')

    rows = table_body.find_all('tr')
    for row in rows:
        cols = row.find_all('td')
        cols = [ele.text.strip() for ele in cols]
        data.append([cols[0], cols[2]])  # Get rid of empty values
    return data


def create_symbols_csv():
    with open("../Data/country_codes.csv", "w", encoding='utf-16', newline='') as f:
        writer = csv.writer(f)
        for country_code in read_country_codes():
            writer.writerow(country_code)


create_symbols_csv()
