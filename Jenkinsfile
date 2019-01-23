pipeline {
    agent any
    options {
        timeout(time: 1, unit: 'HOURS') 
    }
    stages {
        stage ('Init') {
            steps {
                slackSend color: '#0000FF', message: "Started build #${env.BUILD_NUMBER} for branch ${env.BRANCH_NAME}."
                sh 'rm -rf Build/ Archives/'
                sh '/opt/Unity/Editor/Unity -manualLicenseFile /opt/Unity/Editor/Unity_v2018.x.ulf -batchmode -nographics -logfile | true'
            }
        }
        stage ('Build') {
            options {
                timeout(time: 1, unit: 'MINUTES') 
            }
            steps {
                sh 'xvfb-run --auto-servernum --server-args=\"-screen 0 640x480x24:32\" /opt/Unity/Editor/Unity -projectPath . -executeMethod ThePoniesBuilder.RunBuild -batchmode -quit -logfile'
            }
        }
        stage('Archive') {
            steps {
                zip zipFile: 'Archives/ThePonies-Linux.zip', archive: true, dir: 'Build/Linux/'
                zip zipFile: 'Archives/ThePonies-Windows64.zip', archive: true, dir: 'Build/Windows64/'
            }
        }
    }
    post {
        success {
            slackSend color: 'good', message: "Build #${env.BUILD_NUMBER} for branch ${env.BRANCH_NAME} completed successfully."
        }
        unstable {
            slackSend color: 'warning', message: "Build #${env.BUILD_NUMBER} for branch ${env.BRANCH_NAME} completed, but is unstable."
        }
        unsuccessful {
            slackSend color: 'danger', message: "Build #${env.BUILD_NUMBER} for branch ${env.BRANCH_NAME} failed."
        }
        aborted {
            slackSend color: '#888888', message: "Build #${env.BUILD_NUMBER} for branch ${env.BRANCH_NAME} aborted."
        }
    }
}
