// Função para garantir que o Chart.js seja carregado
async function ensureChartJsIsLoaded() {
    // Se o Chart.js já estiver disponível, retorna imediatamente
    if (typeof Chart !== 'undefined') {
        return Promise.resolve();
    }

    // Carrega o Chart.js dinamicamente
    return new Promise((resolve, reject) => {
        const script = document.createElement('script');
        script.src = 'https://cdn.jsdelivr.net/npm/chart.js';
        script.onload = () => {
            console.log('Chart.js carregado com sucesso.');
            resolve();
        };
        script.onerror = () => {
            console.error('Erro ao carregar Chart.js');
            reject(new Error('Falha ao carregar Chart.js'));
        };
        document.head.appendChild(script);
    });
}

export async function renderPieChart(canvasId, title, labels, data) {
    // Garante que o Chart.js esteja carregado antes de continuar
    await ensureChartJsIsLoaded();
    
    const ctx = document.getElementById(canvasId);

    // Evita a duplicação de gráficos no Hot Reload
    if (ctx && ctx.chart) {
        ctx.chart.destroy();
    }

    const backgroundColors = [
        'rgba(59, 130, 246, 0.8)', // blue
        'rgba(16, 185, 129, 0.8)', // emerald
        'rgba(139, 92, 246, 0.8)', // purple
        'rgba(244, 63, 94, 0.8)',  // red
        'rgba(251, 191, 36, 0.8)'  // yellow
    ];
    
    // Adiciona cores de borda com base nas cores de fundo
    const borderColors = backgroundColors.map(color => color.replace('0.8', '1'));

    if (ctx) {
        ctx.chart = new Chart(ctx, {
            type: 'pie',
            data: {
                labels: labels,
                datasets: [{
                    label: title,
                    data: data,
                    backgroundColor: backgroundColors,
                    borderColor: borderColors,
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: title,
                        font: {
                            size: 16
                        }
                    }
                }
            }
        });
    }
};
