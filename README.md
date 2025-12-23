# About Batch Export

Batch Export is a tool to export patient DICOM data from the Eclipse TPS in batch. This tool allows users to query and export data based on patient medical record numbers, and more specifically treatment, courses, and plans. In doing so, users can identify all desired patients within a given database, view all plans associated for a given list of patients, and create a list batch that can later be saved and used to export the associated DICOM files. Ultimately, we have developed a tool for researchers and users to query, organize, and acquire patient DICOM data in an efficient manner. In doing so, challenges faced in radiation oncology and medical physics research such as data scarcity and collecting efficiency may be mitigated.

While this repository contains all the `.dll`s necessary to successfully run the application, the underlying code and mechanisms that perform export are cotained within a "backend" script and done through EvilDICOM and the Eclipse Scripting API (ESAPI). This "backend" was written in a C# script and can be [found here](https://github.com/Varian-MedicalAffairsAppliedSolutions/MAAS-EvilBatchExportDbDaemon). Editing the C# script will require rebuild of it and reimporting all `.dll`s to the folder `BatchTools > LocalMove`.

To read further on Batch Export: JACMP Technical Note - *currently being published*


# Batch Export Quick Start Guide

## 1) Installing the GUI
To install the GUI for Batch Export, please install the built, distributable version of the application found at [this Dropbox link](https://www.dropbox.com/scl/fo/ziz7w7km4k1wispts1iby/ACuXmjbySdElRVnj_fel1bE?rlkey=hyv53ppkghqpnngs97jmstrim&st=56g9exth&dl=0)

## 2) Using the application
The GUI contains 3 tabs: `Build Batch`, `View Current Batch`, and `Export Batch`.

#### 1. Build Batch

This step uses the underlying backend to contact the database and pull the plans of a given list of patients. Begin by clicking `Import` and choosing a `.csv` or `.txt` file where each row (first column in the CSV) contains an MRN to be queried.

Once loaded, all respective plans will appear and by clicking the plan and the `Add to Batch` button, the plan will be added to the list of plans to be exported.

<img src="https://github.com/user-attachments/assets/9898d495-8325-462b-9f7c-ff68998eb85b" alt="openapp" width="600"/>

#### 2. View Current Batch

The list of plans can be viewed in this tab. Changes can be made remove plans from the batch. In additionm, the curated batch can be exported for future use and loaded in using the `Save Batch` and `Load Batch` buttons, respectively. 

<img src="https://github.com/user-attachments/assets/b845cbf1-a16b-40fb-942d-8ab86d258642" alt="openapp" width="600"/>

#### 3. Export Batch

*Note: This step will require proper configuration with the databse and Eclipse, contact respective IT department or help person to obtain IP and ports*

Finally to perform export, the IP, port, and AE title must be configured for the local computer the data will be exported to and the database where the data will be coming from. There are a few options here such as `Anonymize` checkbox, `Export Local`, and `Export Local (no CT)`. 

Once one of the `Export Local` buttons are clicked and the proper configuration is inputted, the user will choose a location where to store all the files and the underlying backend will begin downloading all the plans in an organized structure.

<img src="https://github.com/user-attachments/assets/4cf94ce5-c84d-4771-89c1-22334fae1ec1" alt="openapp" width="600"/>


