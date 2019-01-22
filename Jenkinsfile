pipeline {
    agent any
    stages {
        stage ('Init') {
            steps {
                sh 'rm -rf Build'
                sh '/opt/Unity/Editor/Unity -manualLicenseFile /opt/Unity/Editor/Unity_v2018.x.ulf -batchmode -nographics -logfile | true'
            }
        }
        stage ('Build') {
            steps {
                sh 'mkdir Build; mkdir Build/Linux/; echo "testing, testing" > Build/yay; echo "More testing..." > Build/moreyay;'
                sh 'ls Build/Linux'
            }
        }
        stage('Archive') {
            steps {
                zip zipFile: 'ThePonies-Linux.zip', archive: true, dir: 'Build/Linux/'
            }
        }
    }
}
