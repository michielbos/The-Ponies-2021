# The Ponies - Game
Main repository for The Ponies Unity game project. A parody of The Sims game, with ponies. This readme provides instructions how to setup and use the repository along with git.

## Local Repository Setup
1. Open a new directory on your computer for the repository.
2. Right click -> "Git Bash Here"
3. Type the following commands in command line (replace `<your branch>` with your branch, for instance `master`): 
    * `git init`
    * `git remote add origin https://git.theponies.org/ThePonies/ThePoniesGame.git`
    * `git fetch origin`
    * `git checkout <your branch>`

## Committing, Pushing and Pulling, Misc
To commit:
* `git add .`
* `git commit -am "Your commit message here."`

To push: 
* `git push origin <your branch>`

To pull: 
* `git pull origin <your branch>`

To check the status of your changes:
* `git status`

To list previous commits:
* `git log`