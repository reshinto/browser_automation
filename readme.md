# Browser automation
* Lets you automate the basic typing and clicking actions in the web browser
## How to install
* chromedriver must be installed
### python version
> pipenv install
## Required files
* create secrets.json if authentication is required
```json
{
  "websiteName": {
    "username": "myUsername",
    "password": "myPassword"
  }
}
```
* create elements.json to get elements for actions to execute
  * paste the full xpath as values
```json
{
    "garoon": {
        "login": {
            "userInput": "/html/body/div[4]/div/div[2]/form/div[2]/div[3]/input",
            "pwInput": "/html/body/div[4]/div/div[2]/form/div[2]/div[4]/input",
            "button": "/html/body/div[4]/div/div[2]/form/div[4]/div[2]/input"
        },
        "links": {
            "multiReport": "/html/body/div[2]/div[1]/div/div[1]/div[2]/div[1]/div/span[7]/a",
            "prepareReport": "/html/body/div[2]/div[1]/div/div[2]/table/tbody/tr/td/div/div/div[1]/span[1]/span/a",
            "weeklyReport": "/html/body/div[2]/div[1]/div/div[3]/table/tbody/tr[1]/td[2]/nobr/span/a"
        },
        "weeklyReport": {
            "subjectInput": "/html/body/div[2]/div[1]/div/div[3]/form/table/tbody/tr[1]/td/input",
            "submitButton": "/html/body/div[2]/div[1]/div/div[3]/form/div/span[2]/a"
        }
    }
}
```
* create textfile.txt if you need to submit data to a form
  * set header for each type of input
    * e.g. (inputName)
  * write your data here, multi lines are supported
  * always end input with a (end)
```
(subject)
Weekly Report
(end)
(work)
- did work
- did work again
(end)
```
### For C# version
* save the above 3 files at the same location as the executable file (.exe file)
## TODO
* change this to more OOP oriented
