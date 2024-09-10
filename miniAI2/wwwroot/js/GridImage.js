
function loadTrainingDataIntoListView(data) {
    const listView = document.getElementById("trainingDataListView");

    // Clear the list view
    listView.innerHTML = '';

    data.forEach(imageData => {
        // Create a canvas to draw the image
        const canvas = document.createElement("canvas");
        canvas.width = 28;
        canvas.height = 28;
        const ctx = canvas.getContext("2d");

        // Draw the image using pixel data
        for (let i = 0; i < 28; i++) {
            for (let j = 0; j < 28; j++) {
                const pixelValue = imageData.pixels[i * 28 + j];
                ctx.fillStyle = `rgb(${pixelValue}, ${pixelValue}, ${pixelValue})`;
                ctx.fillRect(j, i, 1, 1);
            }
        }

        // Convert canvas to image
        const img = document.createElement("img");
        img.src = canvas.toDataURL();
        img.alt = "Training Image"; // Optional alt text

        // Add image to the list view
        listView.appendChild(img);
    });
}

// Create the 28x28 grid
let isMouseDown = false;
let isSelecting = true; // Determines whether we're selecting (true) or deselecting (false)

function createPixelGrid() {
    const grid = document.getElementById("pixelGrid");

    for (let i = 0; i < 28 * 28; i++) {
        const cell = document.createElement("div");
        cell.classList.add("pixel-cell");
        cell.dataset.value = "0"; // Default to white (0)

        // Mouse down event to start selection
        cell.addEventListener("mousedown", function (e) {
            isMouseDown = true;
            if (this.dataset.value === "0") {
                isSelecting = true; // Start selecting
                this.style.backgroundColor = "black";
                this.dataset.value = "255";
            } else {
                isSelecting = false; // Start deselecting
                this.style.backgroundColor = "white";
                this.dataset.value = "0";
            }
            e.preventDefault(); // Prevent default dragging behavior
        });

        // Mouse over event for drag selection/deselection
        cell.addEventListener("mouseover", function () {
            if (isMouseDown) {
                if (isSelecting) {
                    this.style.backgroundColor = "black";
                    this.dataset.value = "255";
                } else {
                    this.style.backgroundColor = "white";
                    this.dataset.value = "0";
                }
            }
        });

        // Mouse up event to end selection
        cell.addEventListener("mouseup", function () {
            isMouseDown = false;
        });

        grid.appendChild(cell);
    }

    // Event listener to handle mouse release outside of the grid
    document.body.addEventListener("mouseup", function () {
        isMouseDown = false;
    });
}

function clearGrid() {
    const grid = document.getElementById("pixelGrid");
    const cells = grid.getElementsByClassName("pixel-cell");
    for (let cell of cells) {
        cell.style.backgroundColor = "white";
        cell.dataset.value = "0";
    }
}

// Extract pixel data from the grid into a comma-delimited string
function extractPixelData() {
    const grid = document.getElementById("pixelGrid");
    const cells = grid.getElementsByClassName("pixel-cell");
    const pixelData = [];

    for (let cell of cells) {
        pixelData.push(cell.dataset.value);
    }

    // Output pixel data as a comma-delimited string
    const textarea = document.getElementById("pixelData");
    textarea.value = pixelData.join(",");
}

// Initialize the grid on page load
window.onload = createPixelGrid;

function updateImage() {
    const pixelData = document.getElementById("pixelData").value.split(',').map(Number);
    const canvas = document.getElementById("pixelCanvas");
    const context = canvas.getContext("2d");
    const imageData = context.createImageData(28, 28);

    // Ensure the input data is exactly 784 pixels
    if (pixelData.length === 784) {
        for (let i = 0; i < 784; i++) {
            const pixelValue = pixelData[i];
            const index = i * 4;

            // Set RGBA values: R, G, B, A (Alpha)
            imageData.data[index] = pixelValue; // Red
            imageData.data[index + 1] = pixelValue; // Green
            imageData.data[index + 2] = pixelValue; // Blue
            imageData.data[index + 3] = 255; // Alpha (fully opaque)
        }

        // Update canvas with the new image data
        context.putImageData(imageData, 0, 0);
    } else {
        console.error("Pixel data must contain exactly 784 values.");
    }
}
