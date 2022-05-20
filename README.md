# TclSharp

A TCL implementation for .NET in C#.

## Links

* https://zetcode.com/lang/tcl/lexis/
* https://wiki.tcl-lang.org/page/Dodekalogue
* https://www.tcl-lang.org/man/tcl/TclCmd/Tcl.htm
* https://www.tcl.tk/man/tcl/TclCmd/contents.html


## Implemented Commands

* puts - puts a message to an output.
* set - sets a variable to a value.


## Syntax

### Tokens

* EoF - An end of file/source indicator.
* CommandSeparator - A new line or semicolon.
* Word - A command name or argument.

### BNF

````
script :: [ commands ] EoF .
commands :: command { command-separator command } .
command-separator :: '\n' | ';' .
command :: command-name { command-arguments } .
command-name :: 'A' .. 'Z' | 'a' .. 'z' | '0' .. '9' | '_' .
command-arguments :: word { white-space word } .
word :: simple-word | quoted-word | bracketed-word .
simple-word :: any char but white-space or command-separator .
quoted-word :: '"' { any allowed char } '"' .
bracketed-word :: '{' any allowed char '}' .
white-space :: any white space char but '\n' .
````

Note: A command can be empty, so multiple command separators in a row are allowed. 

Note2: A command name must start with a letter or an underscore.
