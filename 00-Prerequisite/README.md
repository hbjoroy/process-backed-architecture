# Prerequisites 
* K3s
* Helm
* Camunda Platform
* Zeebe command line client
* Camunda Modeler
* MongoDb
* Test project

## K3s and Helm
You need a kubernetes cluster and helm for the series, we use K3s. If you already are running some cluster you should be able to use that.

This series will not cover installing k3s and Helm - use https://k3s.io/ to install k3s, and use https://helm.sh/ to install Helm

The rest of this article assumes you have a working kubernetes cluster available, and helm.

## Camunda Platform

We install the platform based on these [instructions](https://docs.camunda.io/docs/self-managed/platform-deployment/helm-kubernetes/deploy/)

We have placed a modified version of [this file](https://raw.githubusercontent.com/camunda/camunda-platform-helm/main/kind/camunda-platform-core-kind-values.yaml) in our [repository here](camunda-platform-core-kind-values.yaml)

To make this chart work for k3s ```storageClassName``` has been commented out (so it uses the default storage class for the cluster)

```
#    storageClassName: "standard"
```

We first need to add support for the camunda Helm-repository:
```
helm repo add camunda https://helm.camunda.io
helm repo update
```

Now we are ready to run the helm command:
```
kubectl create namespace camunda
helm  install camundaplatform camunda/camunda-platform -f .\camunda-platform-core-kind-values.yaml -n camunda
```

By using ```kubectl get pods -n camunda``` you should now get the list of necesary pods coming in to life:
```
NAME                                             READY   STATUS    RESTARTS   AGE
camundaplatform-connectors-7c58dd7d5c-t89wh      0/1     Running   0          2m44s
camundaplatform-zeebe-gateway-6786b7d767-46nql   1/1     Running   0          2m44s
elasticsearch-master-0                           1/1     Running   0          2m44s
camundaplatform-operate-666b4b668f-5c4nf         1/1     Running   0          2m44s
camundaplatform-tasklist-7bb4cb4f88-msjwk        1/1     Running   0          2m44s
camundaplatform-zeebe-0                          1/1     Running   0          2m44s
```
(Here it looks like the connectors-pod is not working, but we are not using it, so we leave it for now)

The next thing to do is to port forward the operate and the zeebe-gateway so we can use it from our local development.

We do this by using the Rancher Desktop, but you can also do it on the command line using kubectl
```
kubectl port-forward svc/camundaplatform-operate  8081:80 -ncamunda
kubectl port-forward svc/camundaplatform-zeebe-gateway  26500:26500 -ncamunda
```
Operate is now up and running, and available on port 8081.
## Camunda command line client
You should install the ```zbctl``` CLI as instructed here https://docs.camunda.io/docs/apis-tools/cli-client/

Now you should be able to connect to the cluster
```
> zbctl --insecure status

Cluster size: 1
Partitions count: 1
Replication factor: 1
Gateway version: 8.2.3
Brokers:
  Broker 0 - camundaplatform-zeebe-0.camundaplatform-zeebe.camunda.svc:26501
    Version: 8.2.3
    Partition 1 : Leader, Healthy
```
## Camunda modeller
Install the Camunda Modeler as instructed here https://camunda.com/download/modeler/

## MongoDb
Our software will use MongoDb - so we install it in the cluster.

We will use this 

We want to use helm - so the repo needs to be added:
```
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update
```
Then we install mongodb:
```
kubectl create namespace mongodb
helm install -n mongodb mongodb bitnami/mongodb --set auth.enabled=false
```

Follow the instructions from the output of running the helm-command.

Expose mongodb on the outside from the cluster using kubectl or Rancher Desktop
```
kubectl port-forward --namespace mongodb svc/mongodb 27017:27017 
```

You may want to install mongosh by using ```npm i -g mongosh```

## Test project
The test project is located in the folder ```test-project```

Run it, and check that everything is working.
