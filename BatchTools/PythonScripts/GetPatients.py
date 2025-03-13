import pyesapi  # pip install git+https://github.com/VarianAPIs/PyESAPI
import argparse
import json
import os
"""
Example JSON schema for the request.json file
[
  {
    "Comment": null,
    "Courses": [
      {
        "Name": " Prostate1",
        "PatientId": null,
        "ExpandedDisplay": null,
        "Plans": [],
        "ClinicalStatus": null,
        "Comment": null,
        "CompletedDateTime": null,
        "Diagnoses": null,
        "HistoryDateTime": "0001-01-01T00:00:00",
        "HistoryDisplayName": null,
        "HistoryUserName": null,
        "Id": null,
        "Intent": null,
        "StartDateTime": null
      },
      {
        "Name": " 0_Auto3/14/22",
        "PatientId": null,
        "ExpandedDisplay": null,
        "Plans": [
          {
            "UID": null,
            "PatientID": null,
            "ApprovalStatus": null,
            "Comment": null,
            "CreationDateTime": null,
            "CreationUserName": null,
            "HistoryDateTime": null,
            "HistoryUserDisplayName": null,
            "HistoryUserName": null,
            "PlanIntent": null,
            "PlanningApprovalDate": null,
            "PlanningApprover": null,
            "PlanningApproverDisplayName": null,
            "TreatmentApprovalDate": null,
            "TreatmentApprover": null,
            "TreatmentApproverDisplayName": null,
            "Name": " Plan2",
            "Course": null,
            "DoseUID": null,
            "StructureSetUID": null,
            "CTSeriesUID": null,
            "PlanType": null,
            "ExpandedDisplay": null,
            "BulkDataString": null
          }
        ],
        "ClinicalStatus": null,
        "Comment": null,
        "CompletedDateTime": null,
        "Diagnoses": null,
        "HistoryDateTime": "0001-01-01T00:00:00",
        "HistoryDisplayName": null,
        "HistoryUserName": null,
        "Id": null,
        "Intent": null,
        "StartDateTime": null
      }
    ],
    "CreationDateTime": null,
    "DateOfBirth": null,
    "Display": null,
    "ExpandedDisplay": null,
    "FirstName": null,
    "HistoryDateTime": "0001-01-01T00:00:00",
    "HistoryUserDisplayName": null,
    "HistoryUserName": null,
    "Id": "77771111",
    "LastName": null,
    "PrimaryOncologistId": null,
    "Sex": null
  },
  {
    "Id": "03072021",
    "FirstName": null,
    "LastName": null,
    "Display": null,
    "Sex": null,
    "Comment": null,
    "CreationDateTime": null,
    "DateOfBirth": null,
    "HistoryDateTime": "0001-01-01T00:00:00",
    "HistoryUserDisplayName": null,
    "HistoryUserName": null,
    "PrimaryOncologistId": null,
    "ExpandedDisplay": null,
    "Courses": []
  }
]
"""
def getEntirePatient(patient_index, json_request,app):
    """
    This function takes in a patient object and a json request object. It then
    fills in the json request object with the patient's information.

    This function is used when the user does not specifiy any coures or plans.
    Therefore, it is used to get the entire patient information.

    returns the updated json request object     
    """

    patient_id = json_request[patient_index]['Id']

    esapi_patient = app.OpenPatientById(patient_id)
    """
    

    "FirstName": null,
    "LastName": null,
    "Display": null,
    "Sex": null,
    "Comment": null,
    "CreationDateTime": null,
    "DateOfBirth": null,
    "HistoryDateTime": "0001-01-01T00:00:00",
    "HistoryUserDisplayName": null,
    "HistoryUserName": null,
    "PrimaryOncologistId": null,
    "ExpandedDisplay": null,
    "Courses": []
    """


    # fill out the patient information
    json_request[patient_index]['FirstName'] = esapi_patient.FirstName
    json_request[patient_index]['LastName'] = esapi_patient.LastName
    json_request[patient_index]["Sex"] = esapi_patient.Sex
    json_request[patient_index]["Comment"] = esapi_patient.Comment
    json_request[patient_index]["CreationDateTime"] = str(esapi_patient.CreationDateTime)
    json_request[patient_index]["DateOfBirth"] = str(esapi_patient.DateOfBirth)
    json_request[patient_index]["HistoryDateTime"] = str(esapi_patient.HistoryDateTime)
    json_request[patient_index]["HistoryUserDisplayName"] = esapi_patient.HistoryUserDisplayName
    json_request[patient_index]["HistoryUserName"] = esapi_patient.HistoryUserName
    json_request[patient_index]["PrimaryOncologistId"] = esapi_patient.PrimaryOncologistId
    json_request[patient_index]["Display"] = "{}, {} ({})".format(json_request[patient_index]['LastName'], json_request[patient_index]['FirstName'], json_request[patient_index]['Id'])
    json_request[patient_index]["ExpandedDisplay"] = "Patient Name: {} {} Id: ({})\nDOB: {}\n\nProfile Created: {}\nLast Modified: {} ({}) Date: {}".format(json_request[patient_index]['FirstName'], json_request[patient_index]['LastName'], json_request[patient_index]['Id'], json_request[patient_index]['DateOfBirth'], json_request[patient_index]['CreationDateTime'], json_request[patient_index]['HistoryUserDisplayName'], json_request[patient_index]['HistoryUserName'], json_request[patient_index]['HistoryDateTime'])

    # now the courses
    get_all_courses = False
    if json_request[patient_index]['Courses'] == []:
        get_all_courses = True
    else:
        course_requests_list = [request["Id"].strip() for request in json_request[patient_index]['Courses']]
    easpi_courses = esapi_patient.Courses
    for esapi_course_index, course in enumerate(easpi_courses):
        if get_all_courses:
            # create a new course object
            course_object = {}
  
            course_object['Name'] = course.Name
            course_object['PatientId'] = json_request[patient_index]['Id']


            course_object['ClinicalStatus'] = str(course.ClinicalStatus)
            course_object['Comment'] = course.Comment
            course_object['CompletedDateTime'] = str(course.CompletedDateTime)
            # course_object['Diagnoses'] = str(course.Diagnoses)
            course_object['HistoryDateTime'] = str(course.HistoryDateTime)
            course_object['HistoryDisplayName'] = course.HistoryUserDisplayName
            course_object['HistoryUserName'] = course.HistoryUserName
            course_object['Id'] = course.Id

            course_object['Intent'] = course.Intent
            course_object['StartDateTime'] = str(course.StartDateTime)

                #         this.ExpandedDisplay = $"Course Name: {Name} (Patient Id: {PatientId})\nStatus: {ClinicalStatus}\nIntent: {Intent}\nComments: {Comment}" +
                # $"\n\nStartDate: {StartDateTime.ToString()} EndDate: {CompletedDateTime.ToString()}\nLast Modified: {HistoryDisplayName} {HistoryUserName} Date: {HistoryDateTime.ToString()}";
            course_object['ExpandedDisplay'] = "Course Name: {} (Patient Id: {})\nStatus: {}\nIntent: {}\nComments: {}\n\nStartDate: {} EndDate: {}\nLast Modified: {} {} Date: {}".format(course_object['Name'], course_object['PatientId'], course_object['ClinicalStatus'], course_object['Intent'], course_object['Comment'], course_object['StartDateTime'], course_object['CompletedDateTime'], course_object['HistoryDisplayName'], course_object['HistoryUserName'], course_object['HistoryDateTime'])
            # append the course object to the json request
            course_object['Plans'] = []
            json_request[patient_index]['Courses'].append(course_object)
        else:
            if course.Id in course_requests_list:
                # get the index of the course in the json request
                course_index = course_requests_list.index(course.Id)
                course_object = {}
    
                course_object['Name'] = course.Name
                course_object['PatientId'] = json_request[patient_index]['Id']

              
                course_object['ClinicalStatus'] = str(course.ClinicalStatus)
                course_object['Comment'] = course.Comment
                course_object['CompletedDateTime'] = str(course.CompletedDateTime)
                # course_object['Diagnoses'] = str(course.Diagnoses)
                course_object['HistoryDateTime'] = str(course.HistoryDateTime)
                course_object['HistoryDisplayName'] = course.HistoryUserDisplayName
                course_object['HistoryUserName'] = course.HistoryUserName
                course_object['Id'] = course.Id
          
                course_object['Intent'] = course.Intent
                course_object['StartDateTime'] = str(course.StartDateTime)

                course_object['ExpandedDisplay'] = "Course Name: {} (Patient Id: {})\nStatus: {}\nIntent: {}\nComments: {}\n\nStartDate: {} EndDate: {}\nLast Modified: {} {} Date: {}".format(course_object['Name'], course_object['PatientId'], course_object['ClinicalStatus'], course_object['Intent'], course_object['Comment'], course_object['StartDateTime'], course_object['CompletedDateTime'], course_object['HistoryDisplayName'], course_object['HistoryUserName'], course_object['HistoryDateTime'])
                course_object['Plans'] = json_request[patient_index]['Courses'][course_index]['Plans']
                # json_request[patient_index]['Courses'].append(course_object)
                # replace the course object in the json request
                json_request[patient_index]['Courses'][course_index] = course_object
    # now the plans
    #     ExpandedDisplay = $"Plan Name: {Name} Type: {PlanType}\nIntent: {PlanIntent}  \nUID: {UID}\nComment: {Comment}\n\nCreation Date: {CreationDateTime} Created By: {CreationUserName}\n" +
    # $"Last Modified: {HistoryUserDisplayName} {HistoryUserName} Date: {HistoryDateTime}\n" +
    # $"Approval Status: {ApprovalStatus} {PlanningApproverDisplayName} {PlanningApprover} {PlanningApprovalDate}\n" +
    # $"Treatment Approval Date: {TreatmentApprovalDate} {TreatmentApproverDisplayName} {TreatmentApprover}\n" +
    # $"\nDose UID: {DoseUID}\nStructureSet UID: {StructureSetUID}\nCT UID: {CTSeriesUID}\n";
    for course_index, course in enumerate(json_request[patient_index]['Courses']):
        if course['Plans'] == []:
            get_all_plans = True
        else:
            plan_requests_list = [request["Name"].strip() for request in course['Plans']]
            get_all_plans = False

        easpi_plans = [c.PlanSetups for c in easpi_courses if c.Id == course['Id']][0]
        for plan_index, plan in enumerate(easpi_plans):
            if get_all_plans:
                plan_object = {}
                plan_object['UID'] = plan.UID
                plan_object['PatientID'] = plan.Course.Patient.Id
                plan_object['ApprovalStatus'] = str(plan.ApprovalStatus)
                plan_object['Comment'] = plan.Comment
                plan_object['CreationDateTime'] = str(plan.CreationDateTime)
                plan_object['CreationUserName'] = plan.CreationUserName
                plan_object['HistoryDateTime'] = str(plan.HistoryDateTime)
                plan_object['HistoryUserDisplayName'] = plan.HistoryUserDisplayName
                plan_object['HistoryUserName'] = plan.HistoryUserName
                plan_object['PlanIntent'] = plan.PlanIntent
                plan_object['PlanningApprovalDate'] = str(plan.PlanningApprovalDate)
                plan_object['PlanningApprover'] = plan.PlanningApprover
                plan_object['PlanningApproverDisplayName'] = plan.PlanningApproverDisplayName
                plan_object['TreatmentApprovalDate'] = str(plan.TreatmentApprovalDate)
                plan_object['TreatmentApprover'] = plan.TreatmentApprover
                plan_object['TreatmentApproverDisplayName'] = plan.TreatmentApproverDisplayName
                plan_object['Name'] = plan.Id
                plan_object['Course'] = plan.Course.Id
                plan_object['DoseUID'] = plan.Dose.UID if plan.Dose is not None else None
                plan_object['StructureSetUID'] = plan.StructureSet.UID if plan.StructureSet is not None else None
                plan_object['CTSeriesUID'] = plan.StructureSet.Image.Series.UID if plan.StructureSet is not None and plan.StructureSet.Image is not None else None
                plan_object['PlanType'] = str(plan.PlanType)
                plan_object['ExpandedDisplay'] = "Plan Name: {} Type: {}\nIntent: {}\nUID: {}\nComment: {}\n\nCreation Date: {} Created By: {}\nLast Modified: {} {} Date: {}\nApproval Status: {} {} {} {}\nTreatment Approval Date: {} {} {}\n\nDose UID: {}\nStructureSet UID: {}\nCT UID: {}\n".format(plan_object['Name'], plan_object['PlanType'], plan_object['PlanIntent'], plan_object['UID'], plan_object['Comment'], plan_object['CreationDateTime'], plan_object['CreationUserName'], plan_object['HistoryUserDisplayName'], plan_object['HistoryUserName'], plan_object['HistoryDateTime'], plan_object['ApprovalStatus'], plan_object['PlanningApproverDisplayName'], plan_object['PlanningApprover'], plan_object['PlanningApprovalDate'], plan_object['TreatmentApprovalDate'], plan_object['TreatmentApprover'], plan_object['TreatmentApproverDisplayName'], plan_object['DoseUID'], plan_object['StructureSetUID'], plan_object['CTSeriesUID'])
                json_request[patient_index]['Courses'][course_index]['Plans'].append(plan_object)
            else:
                if plan.Name in plan_requests_list:
                    plan_object = {}
                    plan_object['UID'] = plan.UID
                    plan_object['PatientID'] = plan.Course.Patient.Id
                    plan_object['ApprovalStatus'] = str(plan.ApprovalStatus)
                    plan_object['Comment'] = plan.Comment
                    plan_object['CreationDateTime'] = str(plan.CreationDateTime)
                    plan_object['CreationUserName'] = plan.CreationUserName
                    plan_object['HistoryDateTime'] = str(plan.HistoryDateTime)
                    plan_object['HistoryUserDisplayName'] = plan.HistoryUserDisplayName
                    plan_object['HistoryUserName'] = plan.HistoryUserName
                    plan_object['PlanIntent'] = plan.PlanIntent
                    plan_object['PlanningApprovalDate'] = str(plan.PlanningApprovalDate)
                    plan_object['PlanningApprover'] = plan.PlanningApprover
                    plan_object['PlanningApproverDisplayName'] = plan.PlanningApproverDisplayName
                    plan_object['TreatmentApprovalDate'] = str(plan.TreatmentApprovalDate)
                    plan_object['TreatmentApprover'] = plan.TreatmentApprover
                    plan_object['TreatmentApproverDisplayName'] = plan.TreatmentApproverDisplayName
                    plan_object['Name'] = plan.Id
                    plan_object['Course'] = plan.Course.Id
                    plan_object['DoseUID'] = plan.Dose.UID if plan.Dose is not None else None
                    plan_object['StructureSetUID'] = plan.StructureSet.UID if plan.StructureSet is not None else None
                    plan_object['CTSeriesUID'] = plan.StructureSet.Image.Series.UID if plan.StructureSet is not None and plan.StructureSet.Image is not None else None
                    plan_object['PlanType'] = str(plan.PlanType)
                    plan_object['ExpandedDisplay'] = "Plan Name: {} Type: {}\nIntent: {}\nUID: {}\nComment: {}\n\nCreation Date: {} Created By: {}\nLast Modified: {} {} Date: {}\nApproval Status: {} {} {} {}\nTreatment Approval Date: {} {} {}\n\nDose UID: {}\nStructureSet UID: {}\nCT UID: {}\n".format(plan_object['Name'], plan_object['PlanType'], plan_object['PlanIntent'], plan_object['UID'], plan_object['Comment'], plan_object['CreationDateTime'], plan_object['CreationUserName'], plan_object['HistoryUserDisplayName'], plan_object['HistoryUserName'], plan_object['HistoryDateTime'], plan_object['ApprovalStatus'], plan_object['PlanningApproverDisplayName'], plan_object['PlanningApprover'], plan_object['PlanningApprovalDate'], plan_object['TreatmentApprovalDate'], plan_object['TreatmentApprover'], plan_object['TreatmentApproverDisplayName'], plan_object['DoseUID'], plan_object['StructureSetUID'], plan_object['CTSeriesUID'])
                    json_request[patient_index]['Courses'][course_index]['Plans'].append(plan_object)


  
    app.ClosePatient()
    return json_request

 

def main():
    try:
               #change working directory to the directory of this file
        os.chdir(os.path.dirname(os.path.abspath(__file__)))
        current_file = os.path.basename(__file__)
        print("Current File: " + current_file)



        # json test directory
        # if the current file extension is .py: 
      
 
          # tmp_dir = os.path.join(os.path.dirname(os.path.abspath(__file__)),"tmp")
          # print(tmp_dir)
          # dir = os.path.join(tmp_dir,"request.json")
          # output = os.path.join(tmp_dir,"output.json")
    
          # tmp dir is up three directories
        tmp_dir = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))),"tmp")
        print(tmp_dir)
        dir = os.path.join(tmp_dir,"request.json")
        output = os.path.join(tmp_dir,"output.json")
            
        app = pyesapi.CustomScriptExecutable.CreateApplication('python_ex') 
        # open json file
        with open(dir) as f:
            data = json.load(f)

        # the json file deserializes into a nested dictionry
        # the structure of this dictionary is the same as the JSON schema

        # here, we open the patient, if the courses 
        for patient_index, patient in enumerate(data):

              try:
                data = getEntirePatient(patient_index, data,app)
              except Exception as e:

                # write out error to log file
                error_dir = os.path.join(tmp_dir,"error.txt")
                with open(error_dir, 'w') as f:
                    f.write(str(e))
                app.ClosePatient()
                continue






            

        # write to json file
        with open(output, 'w') as outfile:
            json.dump(data, outfile)
             
      



         # script name is used for logging
        # print("Current User: " + app.CurrentUser.ToString())
        # print([p.Id for p in app.PatientSummaries])
    except Exception as e:

        # write out error to log file
        error_dir = os.path.join(tmp_dir,"error.txt")
        with open(error_dir, 'w') as f:
            f.write(str(e))

    finally:
        app.Dispose()

if __name__ == "__main__":


    
    main()