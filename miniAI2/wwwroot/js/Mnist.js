function LoadTrainData() {
    const trainDataPath = document.getElementById('trainDataPath').value;

    fetch('http://localhost:5235/mnist/LoadTrainData', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(trainDataPath)
    })
        .then(response => {
            return response.json();
        })
        .then(result => {
            document.getElementById('trainResult').innerText = result.status;
            loadTrainingDataIntoListView(result.dataList);

        })
        .catch(error => {
            document.getElementById('trainResult').innerText = error;
        });
}
function trainModel() {
    const trainDataPath = document.getElementById('trainDataPath').value;

    fetch('http://localhost:5235/mnist/train', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(trainDataPath)
    })
        .then(response => response.text())
        .then(result => {
            document.getElementById('trainResult').innerText = result;
        })
        .catch(error => {
            document.getElementById('trainResult').innerText = error;
        });
}
function predict() {
    const pixelDataText = document.getElementById('pixelData').value;
    const pixelData = pixelDataText.split(',').map(Number);

    fetch('http://localhost:5235/mnist/predict', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(pixelData)
    })
        .then(response => response.json())
        .then(result => {
            document.getElementById('predictionResult').innerText = result;
        })
        .catch(error => {
            document.getElementById('predictionResult').innerText = error;
        });
}
function forwardPass() {
    const predictionResult = document.getElementById('predictionResult').innerText;

    if (!Array.isArray(predictionResult)) {
        predictedValues = [predictionResult];
    }
    if (predictionResult != "") {
        let res = parseFloat(predictionResult);
        let ret = calculateLoss(res);
    }
}
function calculateLoss(predictedValues) {
    

    // Ask the user for the actual label (as a list for future scalability)
    const actualLabel = parseFloat(prompt("Enter the actual label:"));

    // Here we assume predictedValues is a list and calculate the loss for each predicted value
    let losses = predictedValues.map(predictedValue => Math.pow(predictedValue - actualLabel, 2));

    // Display the calculated losses
    alert("Calculated Losses (MSE): " + losses.join(", "));

    return losses; // Returning the list of losses
}
function calculateGradients(predictedValues, losses) {
    const pixelDataText = document.getElementById('pixelData').value;
    const pixelData = pixelDataText.split(',').map(Number); // Converts pixel data to a list of numbers

    // Prepare the payload with pixel data, predicted values, and losses
    const requestData = {
        pixels: pixelData,
        predictedValues: predictedValues,
        losses: losses
    };

    fetch('http://localhost:5235/mnist/gradient', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(requestData) // Send the payload as JSON
    })
        .then(response => response.json())
        .then(result => {
            document.getElementById('gradientResult').innerText = result;
        })
        .catch(error => {
            document.getElementById('gradientResult').innerText = error;
        });
}

