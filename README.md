# XKeyboard
Custom and Fancy font keyboard application for Windows written in C#

XKeyboard works by intercepting the keyboard on lower-level and modifying the keys forwarding them to windows. 
This lets the application change the keys being sent and act as a 'bridge' between user's keyboard and windows. 

XKeyboard uses dictionary based key-sets (called XFont s) which defines the custom characters for each key on the keyboard. 
When the user persses a key, the program looks for the modified character of the pressed key and forwards it to the windows. 

