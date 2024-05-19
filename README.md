1. Prerequisites

Ensure you have the following installed on your machine:

    Visual Studio Code
    .NET SDK
    Git

2. Clone the Repository Using Terminal

    Open Terminal in VS Code:
        Launch Visual Studio Code.
        Open the integrated terminal by pressing Ctrl+ (backtick) or go to View > Terminal.

    Navigate to the Desired Directory: Use the terminal to navigate to the directory where you want to clone the repository.


cd path/to/your/desired/directory

Clone the Repository: Use the git clone command followed by the URL of your GitHub repository.


git clone https://github.com/lordbinary11/surveyapp.git

Navigate into the Project Folder:


    cd your-repo-name

3. Install Dependencies

    Restore NuGet Packages: Run the following command to restore the NuGet packages required by your project.


    dotnet restore

4. Run the Application

    Build and Run the Project: Use the following command to run your application.


dotnet run or dotnet watch run

Open in Browser: After running the command, you should see output indicating that the application is running and listening on a specific URL (e.g., http://localhost:5000). Open this URL in your web browser to see your application in action.