# CPPinCfilesJSON
From Log to JSON - create a JSON file from Log

Next goals:
- Remove from the c_files_as_CPP all the files that are not .c or .h;
- Add the option for manually input the parsing error that script will find the file name and path.

Future goals:
Create a post scan action that include this script in order to:
- Find the parsing issues;
- Export the JSON and add to project root folder;
- Change the flags in Config.xml to change the CPP parser version (Cx 8.9);
- re-run the scan.

Completed: 
- Finish the implementation in the file path to match the JSON standards; 
- Replace the following Strings in a JSON template file with the results from file name and paths - downloadable file;
  - STRINGPATHTOREPLACE;
  - STRINGFILETOREPLACE.
