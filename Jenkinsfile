pipeline {
    agent any
    stages {
        stage ('Init') {
            steps {
                sh 'rm -rf Build/ Archives/'
                sh '/opt/Unity/Editor/Unity -manualLicenseFile /opt/Unity/Editor/Unity_v2018.x.ulf -batchmode -nographics -logfile | true'
            }
        }
        stage ('Build') {
            steps {
                sh 'mkdir Build; mkdir Build/Linux/; echo "testing, testing" > Build/Linux/yay; echo "More testing..." > Build/Linux/moreyay;'
                sh 'ls Build/Linux'
            }
        }
        stage('Archive') {
            steps {
                zip zipFile: 'Archives/ThePonies-Linux.zip', archive: true, dir: 'Build/Linux/'
            }
        }
    }
}
