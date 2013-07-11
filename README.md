#Chloride Site Generator

This tool generates a complete website based on a template located in the `templates` folder and markdown files located in the `pages` folder which it then puts in a `site` folder.

##Compiling

- Compile the MarkdownSharp DLL
- Compile this project using the provided solution file.
- Run `git submodule update --init`
- Copy all files and folders from `output_env` into the same folder as the EXE

##Credits and Licences for used libraries

This project uses [MarkdownSharp](https://code.google.com/p/markdownsharp/) (version 113), StackOverflow's own Markdown implementation.

You can read all the licences for libraries used in the `LIBRARYLICENCES` file.