# Batch Export

Batch Export is a tool to export patient DICOM data from the Eclipse TPS in batch. This tool allows users to query and export data based on patient medical record numbers, and more specifically treatment, courses, and plans. In doing so, users can identify all desired patients within a given database, view all plans associated for a given list of patients, and create a list batch that can later be saved and used to export the associated DICOM files. Ultimately, we have developed a tool for researchers and users to query, organize, and acquire patient DICOM data in an efficient manner. In doing so, challenges faced in radiation oncology and medical physics research such as data scarcity and collecting efficiency may be mitigated.

While this repository contains all the `.dll`s necessary to successfully run the application, the underlying code and mechanisms that perform export are cotained within a "backend" script and done through EvilDICOM and the Eclipse Scripting API (ESAPI). This "backend" was written in a C# script and can be found in another repository. Editing the C# script will require rebuild of it and reimporting all `.dll`s to the folder `BatchTools > LocalMove`.

To read further on Batch Export: JACMP Technical Note - *currently being published*


# Batch Export Quick Start Guide

## 1) Installing the GUI
The GUI for Batch Export can be installed using 2 methods:
#### Method 1: Cloning the repository
To gain access and version control of all the files, clone the repository by clicking `Code` and using the preferred method of cloning.

<img src="https://github.com/user-attachments/assets/2295830c-d7b4-4a2b-951f-87b26443eaaa" alt="clonerepo" width="600"/>

#### Method 2: Download ZIP
By clicking `Code`, there is also a `Download ZIP` button which can be clicked. This will download all the necessary files **without** version control, meaning that when any new updates are made, all files must be redownloaded.

## 2) Opening the application
Once cloned/downloaded, the following files should be present:

<img src="https://github.com/user-attachments/assets/cc0b0bf8-b072-4414-86c5-efb91ab00ec9" alt="openapp" width="600"/>

Open `BatchExport.sln`, as this will be the entry point into using the GUI (*note: Visual Studio must be downloaded*).

## 3) Using the application
In Visual Studio, to begin using the application press the green play button that says `BatchExportShell` at the top, this will build and open the GUI.

<img src="https://github.com/user-attachments/assets/905688b5-886b-4cc4-ae10-acd7fccc9d15" alt="openapp" width="600"/>

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


