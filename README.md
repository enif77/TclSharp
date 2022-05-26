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
* Text - A bunch of characters but white space or any special chars.

### Script

A script is a list of commands separated by a command separator, which can be the semicolon character (`;`)
or the newline character (LF or `\n`). All OS specific line endings are automatically converted to the LF
character when a script is loaded from a file.

### Commands

Commands are list of words separated by a white space. The first word is used to find a command implementation,
that will use remaining words as its arguments. How it will interpret or use them is up to the command itself.

### Words

...


### BNF

* ::  - definition start.
* .   - end of definition.
* \[] - optional part.
* {}  - 0 or more repetitions.
* \|  - or.
* 'c' - a specific character. 

````
script :: [ commands ] EoF .
commands :: command { command-separator command } .
command-separator :: '\n' | ';' .
command :: word { words-separator word } .
word :: basic-word | quoted-word | bracketed-word .
basic-word :: basic-word-element { basic-word-element } .
basic-word-element :: text | variable-substitution | command-substitution .
text :: any char but white-space or command-separator .
quoted-word :: '"' { any allowed char | variable-substitution | command-substitution } '"' .
bracketed-word :: '{' any allowed char '}' .
white-space :: any unicode white space char but '\n' .
````

Note: A command can be empty, so multiple command separators in a row are allowed. 

Note2: A command name must start with a letter or an underscore.

## TODO

* Handle CRLF as a command separator.
