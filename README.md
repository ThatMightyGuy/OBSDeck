# OBSDeck
## Stream Deck as a HTML page!
Allows you to easily control OBS (or anything you write a driver for!) from a webpage.

## Features
* Write widgets without recompiling in Lua
* Put widgets wherever you want, any size you want, any color, shape you want with HTML and CSS
* Get the same functionality on as many devices as you will ever need
* Update widget HTML as often as you want

## *The Vision™️*
The server acts as a middle ground, serving pages for user control with Lua widgets and running drivers, small bits of software that implement simple C# interfaces, written in Lua, that allow the server to talk to, essentially, anything.

## TODO
* Drivers support
* Drivers for common software
* More actually useful widgets
* `noUpdate` bool property in widget's Lua to save processing and local traffic on static elements
* Better error handling
* Widget interaction - propagate events to their respective Lua logic
* Refactor ticking to only send relevant data to clients

## Known issues
*Read those as part of **TODO***
* The server takes a really long time to stop if it is processing ticking requests from even a single client
* The `WidgetTagHelper.Widgets` dict is not getting cleared of closed connections as it uses the `HttpContext.Connection.Id` as a key, and that `Id` changes by the time the client's SignalR connects
* In particular cases ticking widgets causes an `AccessViolationException` due to the widget's memory being released and the relevant objects disposed of by the time the thread tries to run the Lua on_update function. Might be already fixed by the introduction of locks and minor refactoring - needs more testing
* Client-side JS is very fiddly and stops working when it gets a tick with other clients *(virtually non-existent)* data

## Documentation
*This section is subject to change. A lot.*
### Widget development
A valid widget is any Lua code with `start(guid: str)` and `on_update(): str` functions, that are not local.\
The GUID is used for identificating individual widgets, in case you have several.\
The `start` function is called once, before SignalR connection is established, and even before DOM *begins* loading.\
The `on_update` function is called every tick, unless `noUpdate` is set, in which case it is skipped. It returns an HTML string that will become the new value of `widget.outerHTML`.\
Widgets are added to the page by putting `<widget src="path/to/lua/widget"></widget>` in `Index.cshtml`.

### Driver development
***I don't even know how they will work yet.***

### Page loading order
When the server is started, nothing major happens. Then, when a user tries to connect, the following events occur:
1. The connection is logged
2. The `WidgetTagHelper` class gets calls to replace every `<widget>` tag in `Index.cshtml`
3. It gets the Lua source located at `<widget src`, dofile's it and tries to interpret it as an `IWidget`, creating a `Widget` object on success and immediately running `start()` with a random GUID
5. The `<widget>` tag is replaced by the result of `on_update()`
4. The widget is added to the dictionary `Widgets` with `HttpContext.Connection.Id` as the key
5. The client gets the HTML, downloads SignalR, runs the script to connect to the SignalR hub, sets the timer to tick widgets every second
6. On disconnect, the hub notices the event and triggers widget disposal in order to free precious memory - Lua states are decently large. The disconnect is also logged.

### Quirks
All access to `Widgets` is done through `WidgetTagHelper` as it has the lock, to ensure thread safety.
