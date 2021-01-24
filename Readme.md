This CLI will find public functions on public classes in the assembly specified as the argument. I made this to help with porting and testing existing code that
I was not famililar with. Having the public functions allowed me to create A/B and integration tests to see what worked and didnt after porting.


I added it to the External Tools list in Visual Studio with these values
Arguments  = $(TargetPath)
Initial Directory = $(BinDir)
[X] Use output window


This is bound by the MIT license.