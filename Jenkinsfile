pipeline {
    agent any
    options {
        timeout(time: 1, unit: 'HOURS') 
    }
    stages {
        stage ('Init') {
            steps {
                slackSend color: '#0000FF', message: "Started build ${env.JOB_NAME} #${env.BUILD_NUMBER}. (<${env.BUILD_URL}|Open>)"
                sh 'rm -rf Build/ Archives/'
                sh '/opt/Unity/Editor/Unity -manualLicenseFile /opt/Unity/Editor/Unity_v2019.x.ulf -batchmode -nographics -logfile | true'
            }
        }
        stage ('Build') {
            options {
                timeout(time: 30, unit: 'MINUTES') 
            }
            steps {
                script {
                    if (env.BRANCH_NAME == 'master') {
                        sh 'xvfb-run --auto-servernum --server-args=\"-screen 0 640x480x24:32\" /opt/Unity/Editor/Unity -projectPath . -executeMethod ThePoniesBuilder.RunReleaseBuild -batchmode -quit -logfile'
                    } else {
                        sh 'xvfb-run --auto-servernum --server-args=\"-screen 0 640x480x24:32\" /opt/Unity/Editor/Unity -projectPath . -executeMethod ThePoniesBuilder.RunBuild -batchmode -quit -logfile'
                    }
                }
            }
        }
        stage ('Collect') {
            steps {
                sh 'rm -rf content'
                copyArtifacts(projectName: 'ThePoniesContent/master', filter: 'build/*', target: 'content/');
                sh 'cp content/build/* Build/Linux64/The\\ Ponies/The\\ Ponies_Data/Content/'
                sh 'cp content/build/* Build/Windows64/The\\ Ponies/The\\ Ponies_Data/Content/'
            }
        }
        stage('Archive') {
            steps {
                zip zipFile: 'Archives/ThePonies-Linux64.zip', archive: true, dir: 'Build/Linux64/'
                zip zipFile: 'Archives/ThePonies-Windows64.zip', archive: true, dir: 'Build/Windows64/'
            }
        }
    }
    post {
        success {
            slackSend color: 'good', message: "Build ${env.JOB_NAME} #${env.BUILD_NUMBER} completed successfully. (<${env.BUILD_URL}|Open>)"
        }
        unstable {
            slackSend color: 'warning', message: "Build ${env.JOB_NAME} #${env.BUILD_NUMBER} completed, but is unstable. (<${env.BUILD_URL}|Open>)"
        }
        unsuccessful {
            slackSend color: 'danger', message: "Build ${env.JOB_NAME} #${env.BUILD_NUMBER} failed. (<${env.BUILD_URL}|Open>)"
        }
        aborted {
            slackSend color: '#888888', message: "Build ${env.JOB_NAME} #${env.BUILD_NUMBER} aborted. (<${env.BUILD_URL}|Open>)"
        }
    }
}
