pipeline {
    agent any
    stages {
        stage ('Init') {
            steps {
                script {
                    try {
                        sh '/opt/Unity/Editor/Unity -manualLicenseFile /opt/Unity/Editor/Unity_v2018.x.ulf -batchmode -nographics'
                    } catch (err) {
                        echo err
                    }
                }
            }
        }
        stage ('Build') {
            steps {
                sh 'xvfb-run --auto-servernum --server-args=\"-screen 0 640x480x24:32\" /opt/Unity/Editor/Unity -projectPath . -executeMethod ThePoniesBuilder.RunBuild -batchmode -quit -logfile'
            }
        }
        stage('Archive') {
            steps {
                sh 'echo Archive placeholder.'
            }
        }
    }
}
