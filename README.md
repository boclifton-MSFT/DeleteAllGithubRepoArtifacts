# Delete All Github Repo Artifacts
Console app to delete artifacts from one or more Github Repositories.

## Summary
There is a significant feature gap in Github regarding Actions-created artifacts.  
If you have a free account, you have a pretty low size limit (500MB) and there seems to be no way to manually delete artifacts.
I wrote this script to allow some flexibility in the operation by allowing the user to delete all artifacts in *every* repo or a specific one.

## Instructions
1. Create a [Github Personal Access Token](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token) with 'repo' permission
2. Clone the repo to your local machine
3. Build the project and publish, either with Visual Studio (right click the project and select 'Publish') or use the `dotnet publish` command
4. Run the .exe file in the `/publish` directory from a Terminal window with arguments in this order:
    - Personal Access Token
    - Github User Name (not email address, mine is boclifton-MSFT)
    - (optional) the name of a single repo to delete artifacts from. If this is omitted, the script will target **_every_** repo and delete **_all_** your artifacts.  The script will prompt to confirm before deleting from all repos.
4. ???
5. Profit!
