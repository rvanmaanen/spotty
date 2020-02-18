# Spotty
A project to extend some Spotify functionality. At this point there are 2 goals:
- It should be able to automatically take over a current Spotify session and continue playing on the machine the app is installed on. It there isn't a current session, hopefully it can continue playing at the point where you stopped and if that fails it should start something that can be configured.
- It should provide an API that will provide easy integration for a Google Home so you can control Spotify desktop by voice. Any custom Google Home related stuff needed will be included here. 

# TODO
- Deal with unsuccessfull login callback
- Make use of refresh token
- Add random state parameter when logging in
- Move URLs to appsettings
- Add errorhandling when calling Spotify API