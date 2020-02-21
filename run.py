import json
from datetime import date
import datetime
from functools import partial
from selenium import webdriver


def readFile(_file):
    with open(_file) as f:
        return json.load(f)


loginData = readFile("secrets.json")
elements = readFile("elements.json")


url = elements["url"]
browser = webdriver.Chrome()
browser.get(url)

inputs = {
    "(subject)": {
        "bool": False,
        "text": ""
    },
    "(work)": {
        "bool": False,
        "text": ""
    },
    "(problems)": {
        "bool": False,
        "text": ""
    },
    "(plans)": {
        "bool": False,
        "text": ""
    },
    "(info)": {
        "bool": False,
        "text": ""
    },
    "(impression)": {
        "bool": False,
        "text": ""
    },
}


def readWeeklyReport(_file):
    paraStart = False
    current = ""
    with open(_file) as f:
        for line in f:
            lineLength = len(line) - 1
            _line = line[:lineLength]
            if _line == "(end)" or line == "(end)":
                paraStart = False
                inputs[current]["bool"] = False
                stringValue = inputs[current]["text"]
                length = len(stringValue)
                inputs[current]["text"] = stringValue[:length-1]
            if paraStart:
                if inputs[current]["bool"]:
                    inputs[current]["text"] += line
            if _line in inputs:
                paraStart = True
                current = _line
                inputs[_line]["bool"] = True


def getDate():
    today = date.today()
    year = today.year
    month = today.strftime("%B")
    base = datetime.datetime.today()
    date_list = [base - datetime.timedelta(days=x) for x in range(5)]
    day = f"{date_list[len(date_list)-1].strftime('%d')}-{date_list[0].strftime('%d')}"
    return f"({day} {month} {year})"


def getInfo(data, site, auth_type):
    return data[site][auth_type]


def getElement(xPath):
    return browser.find_element_by_xpath(xPath)


def runTask(task):
    while True:
        isLoading = False
        try:
            task()
        except:
            isLoading = True
        if isLoading is False:
            break


def clickAction(*args):
    while True:
        isLoading = False
        try:
            getElement(elements[args[0]][args[1]][args[2]]).click()
        except:
            isLoading = True
        if isLoading is False:
            break


def runLoginTasks():
    loginTasks = [
        partial(getElement(elements["garoon"]["login"]["userInput"]).send_keys, getInfo(loginData, "garoon", "username")),
        partial(getElement(elements["garoon"]["login"]["pwInput"]).send_keys, getInfo(loginData, "garoon", "password")),
    ]
    for task in loginTasks:
        runTask(task)


def runWeeklyReportTasks():
    """
    Subject: Weekly Report (dd-dd Month yyyy)
    """
    weeklyReportTasks = [
        partial(getElement(elements["garoon"]["weeklyReport"]["subjectInput"]).clear),
        partial(getElement(elements["garoon"]["weeklyReport"]["subjectInput"]).send_keys, f"{inputs['(subject)']['text']} {getDate()}"),
        partial(getElement(elements["garoon"]["weeklyReport"]["workThisWeekInput"]).send_keys, inputs["(work)"]["text"]),
        partial(getElement(elements["garoon"]["weeklyReport"]["problemsInput"]).send_keys, inputs["(problems)"]["text"]),
        partial(getElement(elements["garoon"]["weeklyReport"]["PlanForNextWeekInput"]).send_keys, inputs["(plans)"]["text"]),
        partial(getElement(elements["garoon"]["weeklyReport"]["infoToManagerInput"]).send_keys, inputs["(info)"]["text"]),
        partial(getElement(elements["garoon"]["weeklyReport"]["impressionInput"]).send_keys, inputs["(impression)"]["text"]),
    ]
    for task in weeklyReportTasks:
        runTask(task)
    

def run():
    tasks = [
            partial(runLoginTasks),
            partial(clickAction, "garoon", "login", "button"),
            partial(clickAction, "garoon", "links", "multiReport"),
            partial(clickAction, "garoon", "links", "prepareReport"),
            partial(clickAction, "garoon", "links", "weeklyReport"),
            partial(runWeeklyReportTasks),
            partial(clickAction, "garoon", "weeklyReport", "notiForBossSelect"),
            partial(clickAction, "garoon", "weeklyReport", "notiAddButton"),
            partial(clickAction, "garoon", "weeklyReport", "confirmDetailsButton")
    ]
    for task in tasks:
        task()


def submit():
    #clickAction("garoon", "weeklyReport", "confirmDetailsButton")
    clickAction("garoon", "weeklyReport", "submitButton")
    browser.quit()



if __name__ == "__main__":
    readWeeklyReport(r"C:\Users\user\Desktop\weeklyReport.txt")
    run()
