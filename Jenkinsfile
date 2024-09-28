pipeline {
    agent any
    stages {
        stage('Checkout') {
            steps {
                // Checkout the code from GitHub
                git url: 'https://github.com/RajshDev/Jentest.git', branch: 'main'
            }
        }
        stage('Print Message') {
            steps {
                echo 'king'
            }
        }
    }
}
