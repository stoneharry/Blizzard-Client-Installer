// TheScript.js
// Author: Jaime Wood
// Created on: September 3rd, 2004
// Purpose: This handles all of the menu actions and navigation of the help pages.
//          It's function was based off of Collin Smith's scripts used in the previous
//          games' ReadMe, but no original code remains.
// Lasted Edited: September 15, 2004
// Reason Edited: 9-15 Updated to accomodate the menu system implemented 
//                9-5 Added additional error checking, and included comments

// Declare all of the roll-over images for precaching purposes
imgPlatformOn = new Image();
imgPlatformOff = new Image();
imgReadMeOn = new Image();
imgReadMeOff = new Image();
imgTroubleshootingOn = new Image();
imgTroubleshootingOff = new Image();
imgContactInfoOn = new Image();
imgContactInfoOff = new Image();
imgWebsiteOn = new Image();
imgWebsiteOff = new Image();

readmeOn = new Image();
readmeOff = new Image();

pcOn = new Image();
pcOff = new Image();

macOn = new Image();
macOff = new Image();

troubleshootingOn = new Image();
troubleshootingOff = new Image();

contactinfoOn = new Image();
contactinfoOff = new Image();

websiteOn = new Image();
websiteOff = new Image();

// The locations of all of the roll-over images
pcOn.src = "../Images/buttons/pc-over.jpg";
pcOff.src = "../Images/buttons/pc.jpg";

macOn.src = "../Images/buttons/mac-over.jpg";
macOff.src = "../Images/buttons/mac.jpg";

readmeOn.src = "../Images/buttons/readme-over.gif";
readmeOff.src = "../Images/buttons/readme.gif";

troubleshootingOn.src = "../Images/buttons/troubleshooting-over.gif";
troubleshootingOff.src = "../Images/buttons/troubleshooting.gif";

contactinfoOn.src = "../Images/buttons/contactinfo-over.gif";
contactinfoOff.src = "../Images/buttons/contactinfo.gif";

websiteOn.src = "../Images/buttons/website-over.gif";
websiteOff.src = "../Images/buttons/website.gif";

imgPlatformOn.src = "../Images/Nav/Nav_01-On.jpg";
imgPlatformOff.src = "../Images/Nav/Nav_01.jpg";
imgReadMeOn.src = "../Images/Nav/Nav_02-On.jpg";
imgReadMeOff.src = "../Images/Nav/Nav_02.jpg";
imgTroubleshootingOn.src = "../Images/Nav/Nav_03-On.jpg";
imgTroubleshootingOff.src = "../Images/Nav/Nav_03.jpg";
imgContactInfoOn.src = "../Images/Nav/Nav_04-On.jpg";
imgContactInfoOff.src = "../Images/Nav/Nav_04.jpg";
imgWebsiteOn.src = "../Images/Nav/Nav_05-On.jpg";
imgWebsiteOff.src = "../Images/Nav/Nav_05.jpg";

//These are the default values which decide what pages the user is on
strPlatform = "PC";             // The platform the user is using or has choosen.  
strTopic = "ReadMe";            // The topic the user is currently in
btnLock = document.images[9];   // The button that represents the section the user is in.. it remains lit
platformLock = document.images[4];   // The button that represents the platform the user is in.. it remains lit

// Find out what page they are on..
var strLocation = new String (parent.location);
var aryLocation = strLocation.split("/");

strLocation = aryLocation[aryLocation.length - 1];

// If thy are running on a mac, then default to the mac platform 
if (navigator.userAgent.indexOf('Mac') != -1)
{
    changeMacExceptContent();
    parent.Topic.location = "../" + strTopic + "/(" + strPlatform + ")" + strTopic + "menu.html";
    if (strLocation == "Requirements.html")
	    parent.content.location = "../" + strTopic + "/(" + strPlatform + ")SystemRequirements.html";
    else
    	parent.content.location = "../" + strTopic + "/(" + strPlatform + ")Foreword.html";
}

// Function: rollover(imgCurrent,bAction)
// Purpose: Takes the passed button, and turns it on or off based on the action boolean
// Input: imgCurrent        - The image that is changing (on or off)
//        bAction           - Boolean that holds 0 for on or 1 for off
function rollover(imgCurrent, bAction){
    // If it is true then turn it on
	if (bAction)
	    imgCurrent.src = eval(imgCurrent.id + "On.src");     
	// Otherwise if the button is not locked, turn it off
	else
	    if (imgCurrent != btnLock && imgCurrent != platformLock)
	    {
	        imgCurrent.src = eval(imgCurrent.id + "Off.src");
	    }
}


// Function: changePC()
// Purpose: This function will take the user to the PC version of the readme
function changePC(){
    // If it is already on PC do nothing
    if (strPlatform != "PC")
    {
        // Set the images to display the current platform
        document.images[4].src = pcOn.src;
        document.images[6].src = macOff.src;
        
        // Change the platform to PC 
        strPlatform = "PC";
        
        // Set the platformLock 
        platformLock = document.images[4];
        
        // Change the menu on the left so that it shows the platform correct menu of the current topic
        parent.Topic.location = "../" + strTopic + "/(" + strPlatform + ")" + strTopic + "menu.html";
        // The body on the right changes to a splash page
        parent.content.location = "splash.html";
    }

}

// Function: changeMac()
// Purpose: This function will take the user to the Mac version of the readme
function changeMac(){
    // If it is already on Mac do nothing
    if (strPlatform != "Mac")
    {
		changeMacExceptContent();
        parent.content.location = "splash.html";
    }

}

// Function: changeMacExceptContent()
// Purpose: This function will take the user to the Mac version of the readme
function changeMacExceptContent(){
    // If it is already on Mac do nothing
    if (strPlatform != "Mac")
    {
        // Set the images to display the current platform
        document.images[6].src = macOn.src;
        document.images[4].src = pcOff.src;
        
        // Change the platform to Mac
        strPlatform = "Mac";
        
        // Set the platformLock 
        platformLock = document.images[6];
        
        // Change the menu on the left so that it shows the platform correct menu of the current topic
        parent.Topic.location = "../" + strTopic + "/(" + strPlatform + ")" + strTopic + "menu.html";
    }

}

// Function: changePage(strPage, Button)
// Purpose: This function will take the user to a differnt page of the readme.  Menu is loaded to 
//          the left and a splash screen is loaded to the right
// Input: strPage - The topic to change to
//        Button - The button that corresponds to the page we are going to... the button that was clicked
function changePage(strPage, Button){
    // If the used the website button, then open a new window with the website
    if (strPage == 'Website')
       window.open ("http://www.worldofwarcraft.com");   
    else
    {       
        // Change the topic to the page we are going to
        strTopic = strPage;
        
        // Change the menu to the one for the topic and throw up a splash page in the body pane
        parent.Topic.location = "../" + strTopic + "/(" + strPlatform + ")" + strTopic + "menu.html";
        parent.content.location = "splash.html";
            
        // Make sure they are not clicking the locked button if so, do not bother with turning the old off   
        if (btnLock != Button)
        {
            // Turn off the previous button
            btnLock.src = eval(btnLock.id + "Off.src");
            // Lock the new button
            btnLock = Button;
        }
    }
    
}
