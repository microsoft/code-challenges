# microsoft.docs

Welcome to Build Hand On Labs 2017.

Please put individual labs into the Labs directory with an appropriate name.

e.g. Labs/Azure Search

Any notes about the labs should be placed in the Project Definitions directory.

e.g. Project Definitions/BUILD Hands-on-labs 2017.md

## Building a Lab

Create your lab and write a lab guide in mark down.

Add a task to .vscode/tasks.json to "compile" this lab

e.g.

```json
"taskName": "Compile Azure Search Lab",
"args": ["-i", "Labs/Azure Search/hands-on-lab.md", "-o", "Labs/Azure Search/hands-on-lab.html"],
"suppressTaskName": true,
"isBuildCommand": true
```

To run this task to generate the html in VS Code

Press CTRL + P

```powershell
task Compile Azure Search Lab
```

