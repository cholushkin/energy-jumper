----- MVHC pattern
Slightly based on http://engineering.socialpoint.es/MVC-pattern-unity3d-ui.html

##### Model
Holds no view data nor view state data.
Is accessed by the Controller and other Models only
Will trigger events to notify external system of changes.

##### View 
The View is just something that will be rendered in the screen. 
* View knows nothing about other parts of the project, either data or logic. It has no access to Controller, EventHandler, Model.
* Controller provides all needed data to View. Controller must explicitly tell the view what to display, what animation to play, what state to change to, etc.
* View could be manipulated by other views
* Control user inputs 
* Handle references to elements needed for drawing (Textures, FXs, etc)
* Perform Animations
* Layouts and different appearances. Different inputs for one widget (for example gamepad version of View or touch version of same View)
* Spawn events of internal representation:
    * state changes events
    * input related events

--- Controller
The Controller is the link between the Model and the View . It holds the state of the View and updates it depending on that state and on external events:
Holds the application state needed for that view
* Controls view flow
* Gets information from Model and EventHandler
* spawn events of control domain (like )

--- EventHandler
EventHandler responsible for the context where this control is working
* Receives events from DataModel and from domain (other UI's control events)


----- Flows
Switches between screens occurs in game states. Screens just notify current game state
by events or directly be switching game state.


--- Close group example: 
* Caption control publish control event EventCaptionClick
* UIScreenPalette handles EventCaptionClick
	* switch state to GroupSelect
		* publish EventElementsPanelEnter
		* publish EventGroupPanelExit

--- Open group example:
* GroupButton publish internal event EventGroupButtonClick
* GroupsPanel handles
	* calls GroupSelect and publish control event EventGroupSelected
* Elements panel handles EventGroupSelected
	* creates elements
* UIScreenPalette handles EventGroupSelected
	* switches UIScreenPalette navigation state to ElementSelect
		* publish EventGroupsPanelExit
		* publish EventElementsPanelEnter