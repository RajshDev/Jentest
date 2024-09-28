pipeline {
    agent any
    stages {
        stage('Create Text File') {
            steps {
                script {
                    def filePath = "C:\\Users\\User\\Desktop\\projects\\firstone.txt"
                    def fileContent = "I want to become a developer engineer in Singapore"
                    
                    writeFile file: filePath, text: fileContent
                }
            }
        }
    }
}
