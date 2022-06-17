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
* CommandSeparator - The new line or the semicolon character.
* Word - A command name or argument.
* Text - A bunch of characters but white space or any special chars.
* VariableSubstitution - A $variable or a ${variable} substitution.
* CommandSubstitution - A \[command args...] substitution.

### Script

A script is a list of commands separated by a command separator, which can be the semicolon character (`;`)
or the newline character (LF or `\n`). All OS specific line endings are automatically converted to the LF
character when a script is loaded from a file.

### Commands

Commands are list of words separated by a white space. The first word is used to find a command implementation,
that will use remaining words as its arguments. How it will interpret or use them is up to the command itself.

A command can be empty, so multiple command separators in a row are allowed. 

### Words

Words are non-white characters separated by white characters. Can be grouped together using double quotes (`" ... "`) or
brackets (`{ ... }`).

All words can contain substitutions - variable, escaped characters or command substitution. Substitutions in bracketed words
are not processed.

### Comments.

If a hash character (`#`) appears at a point where Tcl is expecting the first character of the first word of a command,
then the hash character and the characters that follow it, up through the next newline, are treated as a comment and ignored.
The comment character only has significance when it appears at the beginning of a command.

### BNF

* ::  - a definition start.
* .   - the end of a definition.
* \[] - an optional part.
* {}  - 0 or more repetitions.
* ()  - a group of elements.
* \|  - or.
* ..  - a range (from '0' to '9').
* 'c' - a specific character.
* EoF - the end of the file marker (int -1). 

````
script :: [ commands | comment ] EoF .
commands :: command { command-separator command } .
command-separator :: '\n' | ';' .
command :: ( word { words-separator word } ) | comment .
comment :: '#' any chars but '\n' or EoF .
word :: basic-word | quoted-word | bracketed-word .
basic-word :: basic-word-element { basic-word-element } .
basic-word-element :: text | variable-substitution | command-substitution .
text :: any char but white-space or command-separator .
variable-substitution :: '$' variable-name | '$' '{' text '}' .
variable-name :: digits | alphabet-chars | '_' .
digits :: '0' .. '9' .
alphabet-chars :: 'a' .. 'z' | 'A' .. 'Z' .
command-substitution :: '[' any allowed char | escaped-command-substitution-chars ']' .
escaped-command-substitution-chars :: '\[' | '\]' .
quoted-word :: '"' { any allowed char | escaped-char | variable-substitution | command-substitution } '"' .
escaped-char :: '\' char .
bracketed-word :: '{' any allowed char | escaped-bracketed-word-char '}' .
escaped-bracketed-word-char :: '\{' | '\}' .
white-space :: any unicode white space char but '\n' .
````


## TODO

...
