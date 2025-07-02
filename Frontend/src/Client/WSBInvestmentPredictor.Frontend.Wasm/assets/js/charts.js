// Global variable to store chart instances
window.chartInstances = {};

// Function to check if Chart.js is loaded
window.isChartJsLoaded = function() {
    return typeof Chart !== 'undefined';
};

// Function to render pie chart
window.renderPieChart = function (canvasId, chartData) {
    try {
        console.log('Rendering pie chart for:', canvasId);
        console.log('Chart data:', chartData);
        
        // Check if Chart.js is loaded
        if (!window.isChartJsLoaded()) {
            console.error('Chart.js is not loaded. Please ensure the Chart.js script is included.');
            return;
        }
        
        // Destroy existing chart if it exists
        if (window.chartInstances[canvasId]) {
            console.log('Destroying existing chart for:', canvasId);
            try {
                window.chartInstances[canvasId].destroy();
            } catch (error) {
                console.warn('Error destroying existing chart:', error);
            }
            delete window.chartInstances[canvasId];
        }

        const canvas = document.getElementById(canvasId);
        if (!canvas) {
            console.error('Canvas element not found:', canvasId);
            return;
        }

        const ctx = canvas.getContext('2d');
        if (!ctx) {
            console.error('Could not get 2D context for canvas:', canvasId);
            return;
        }
        
        // Validate chart data
        if (!chartData || !chartData.labels || !chartData.datasets) {
            console.error('Invalid chart data structure:', chartData);
            return;
        }
        
        // Create new chart
        window.chartInstances[canvasId] = new Chart(ctx, {
            type: 'pie',
            data: chartData,
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            padding: 20,
                            usePointStyle: true
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                const label = context.label || '';
                                const value = context.parsed;
                                const total = context.dataset.data.reduce((a, b) => a + b, 0);
                                const percentage = ((value / total) * 100).toFixed(1);
                                return `${label}: ${value} (${percentage}%)`;
                            }
                        }
                    }
                }
            }
        });
        
        console.log('Chart created successfully for:', canvasId);
    } catch (error) {
        console.error('Error creating chart for', canvasId, ':', error);
        console.error('Chart data was:', chartData);
    }
};

// Function to destroy chart
window.destroyChart = function (canvasId) {
    try {
        if (window.chartInstances[canvasId]) {
            console.log('Destroying chart for:', canvasId);
            window.chartInstances[canvasId].destroy();
            delete window.chartInstances[canvasId];
        }
    } catch (error) {
        console.error('Error destroying chart for', canvasId, ':', error);
    }
};

// Chart.js configuration and utilities
window.charts = {
    renderPieChart: function (canvasId, data, options = {}) {
        try {
            const ctx = document.getElementById(canvasId);
            if (!ctx) {
                console.error(`Canvas with id '${canvasId}' not found`);
                return null;
            }

            // Destroy existing chart if it exists
            if (window.charts.currentPieChart) {
                try {
                    window.charts.currentPieChart.destroy();
                } catch (error) {
                    console.warn('Error destroying existing chart:', error);
                }
            }

            const defaultOptions = {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            padding: 20,
                            usePointStyle: true
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                const label = context.label || '';
                                const value = context.parsed;
                                const total = context.dataset.data.reduce((a, b) => a + b, 0);
                                const percentage = ((value / total) * 100).toFixed(1);
                                return `${label}: ${value} (${percentage}%)`;
                            }
                        }
                    }
                },
                onClick: function (event, elements) {
                    if (elements.length > 0) {
                        const index = elements[0].index;
                        const label = data.labels[index];
                        const value = data.datasets[0].data[index];
                        
                        // Emit custom event for filtering
                        const filterEvent = new CustomEvent('pieChartFilter', {
                            detail: {
                                category: label,
                                value: value,
                                index: index
                            }
                        });
                        document.dispatchEvent(filterEvent);
                    }
                }
            };

            const config = {
                type: 'pie',
                data: data,
                options: { ...defaultOptions, ...options }
            };

            window.charts.currentPieChart = new Chart(ctx, config);
            return window.charts.currentPieChart;
        } catch (error) {
            console.error('Error in renderPieChart:', error);
            return null;
        }
    },

    // Method to clear current filter
    clearFilter: function () {
        const clearEvent = new CustomEvent('pieChartFilter', {
            detail: {
                category: null,
                value: null,
                index: -1
            }
        });
        document.dispatchEvent(clearEvent);
    },

    // Method to handle filter events and communicate with Blazor
    setupFilterHandler: function (dotNetHelper) {
        document.addEventListener('pieChartFilter', function(event) {
            try {
                // Check if dotNetHelper still exists and is valid
                if (!dotNetHelper || typeof dotNetHelper.invokeMethodAsync !== 'function') {
                    console.warn('DotNetHelper is no longer valid, skipping method invocation');
                    return;
                }
                
                const category = event.detail.category;
                dotNetHelper.invokeMethodAsync('OnPieChartCategorySelected', category)
                    .catch(error => {
                        console.warn('Error invoking Blazor method:', error);
                    });
            } catch (error) {
                console.error('Error in filter handler:', error);
            }
        });
    }
}; 