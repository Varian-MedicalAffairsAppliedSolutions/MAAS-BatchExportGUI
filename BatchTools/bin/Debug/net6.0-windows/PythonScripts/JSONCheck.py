import pyesapi  # pip install git+https://github.com/VarianAPIs/PyESAPI

def main():
    try:
        app = pyesapi.CustomScriptExecutable.CreateApplication('python_ex')  # script name is used for logging
        # print("Current User: " + app.CurrentUser.ToString())
        # print([p.Id for p in app.PatientSummaries])
        patient = app.OpenPatientById('77771111')
        
        print(patient.Id)

    finally:
        app.Dispose()
# import json
# def main():
#     # some JSON:
#     x =  {"a":"123","b":"456"}

#     # parse x:
#     y = json.dumps(x)
#     print(y)
if __name__ == "__main__":
    main()