pipeline {
    agent any
    stages {
        stage ('Init') {
            steps {
                sh 'echo Init placeholder.'
            }
        }
        stage ('Build') {
            steps {
                sh 'xvfb-run --auto-servernum --server-args=\\\'-screen 0 640x480x24:32\\\' /opt/Unity/Editor/Unity -projectPath /home/michiel/ThePoniesGame/ -executeMethod ThePoniesBuilder.RunBuild -batchmode -quit -logfile'
            }
        }
        stage('Archive') {
            steps {
                sh 'echo Archive placeholder.'
            }
        }
    }
}
