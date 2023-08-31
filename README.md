<p align="left">
   <img src="https://img.shields.io/badge/.NET-v4.72+%20-dc9799" alt="Technology">
   <img src="https://img.shields.io/badge/Licence-GPL 3.0%20-a37fc0" alt="Technology">
   <img src="https://img.shields.io/badge/Visual Studio-2022%20-6e5be1" alt="Technology">
  <a href="https://codebeat.co/projects/github-com-flourishawk-rating_calculator-main">
  <img alt="codebeat badge" src="https://codebeat.co/badges/ab3b248e-a4fe-48c3-8632-82f998710bc8"/>
  </a>
</p>
      <p align="center">

</p>


## About this console app

This console application will allow you to build an admission rating based on the uploaded .csv files of specialties, 
which will allow you to assess your chances of getting a state-funded place.

## Documentation

### Input Data

  This application can calculate the ranking position of students in a specialty within a single university and display a list of applicants. 
  As a bonus, at the end you can get a ranking list by industry (specialty number) with a percentage position, which will allow you to estimate the possibility of receiving scholarships.
  To get started, you need to place files from the specialties you are interested in in the Import .csv folder (the more files you have, the more accurate the application will be).
#### The .csv format imposes its own syntactic restrictions, namely:
    - data starts immediately from the first line, without a header;
    - the .csv file format must be UTF8;
    - the data should look exactly like this
   <p align="center">
   <img src="https://i.ibb.co/1GBhjmz/example-rating.png" alt="Rating Data" align="center">
   </p>

(The first line in this case exists only for additional information. It should not be present in the data in real use)
   
### Export Data

  You can get the necessary data in the Export folder in .csv format
