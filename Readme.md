# Welcome to DRS Race Manager Project.
The goal of this project is to have a fully functioning Race Manager System for LFS in a local installation.

#Key Functions of the program
- Enable local logging of selected Race Data into a local MySQL Database
    - Drivername (not username)
    - Best Lap Time for all Cars And Tracks. (84 Track configs multiplied by 20 cars 1680 fields pr. Driver)
    - Finish statistics. (Number of 1, 2, 3, 4, 5, 6, 7 and 8 places)
    - Number of KM driven.
    - Race number (Starting from 1, going on forever)
    
- InSim Interface functions
    - Menu For Points assignment.
    
- Insim Display on Game Screen Properties.
    - Insim should Display Distance driven, Name, Points, Current Ranking, Best Laptime on Lap/car, WR on Lap/car
    - When race is finished, a list of drivers in that race should be displayed, with the corresponding Points, sorted by points.
 
- InSim local program functions
    - Reset points for all drivers in DB
    - Change points of selected driver to Anything.

- Additional functionality.
    - Create a c# Display clientprogram, that pulls data from the SQL base and displays on screens.
    - Create a Web plugin, that pulls data from SQL base and displays on web page.
    - Create a Facebook app that shows data from SQL base
    - Create a IOS app for Drammen Racing Senter that connects to SQL
    - Create a Android app for Drammen Racing Senter that connects to SQL
