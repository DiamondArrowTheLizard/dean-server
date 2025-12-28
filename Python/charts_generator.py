import sys
import json
import matplotlib.pyplot as plt
import matplotlib
from matplotlib.backends.backend_pdf import PdfPages
import numpy as np

def read_input_data():
    input_bytes = sys.stdin.buffer.read()
    input_str = input_bytes.decode('utf-8-sig').strip()
    if not input_str:
        raise ValueError("No input data received")
    return json.loads(input_str)

def create_charts(data, output_png, output_pdf):
    matplotlib.use('Agg')
    
    fig, axes = plt.subplots(2, 2, figsize=(15, 10))
    fig.suptitle('Статистика деканата', fontsize=16, fontweight='bold')
    
    ax1 = axes[0, 0]
    courses = list(data['StudentsByCourse'].keys())
    counts = list(data['StudentsByCourse'].values())
    colors = plt.cm.Set3(np.linspace(0, 1, len(courses)))
    
    bars = ax1.bar(courses, counts, color=colors, edgecolor='black')
    ax1.set_xlabel('Курс')
    ax1.set_ylabel('Количество студентов')
    ax1.set_title('Распределение студентов по курсам')
    ax1.grid(True, alpha=0.3)
    
    for bar, count in zip(bars, counts):
        ax1.text(bar.get_x() + bar.get_width()/2, bar.get_height() + 0.5,
                str(count), ha='center', va='bottom', fontsize=9)
    
    ax2 = axes[0, 1]
    faculties = list(data['PerformanceByFaculty'].keys())
    avg_marks = list(data['PerformanceByFaculty'].values())
    colors2 = plt.cm.Paired(np.linspace(0, 1, len(faculties)))
    
    bars2 = ax2.bar(faculties, avg_marks, color=colors2, edgecolor='black')
    ax2.set_xlabel('Факультет')
    ax2.set_ylabel('Средний балл')
    ax2.set_title('Успеваемость по факультетам')
    ax2.grid(True, alpha=0.3)
    
    if len(faculties) > 0:
        ax2.set_xticks(range(len(faculties)))
        ax2.set_xticklabels(faculties, rotation=45, ha='right')
    
    for bar, mark in zip(bars2, avg_marks):
        ax2.text(bar.get_x() + bar.get_width()/2, bar.get_height() + 0.05,
                f'{mark:.2f}', ha='center', va='bottom', fontsize=9)
    
    ax3 = axes[1, 0]
    basis_labels = {'budget': 'Бюджет', 'contract': 'Контракт'}
    basis_keys = list(data['EducationBasisDistribution'].keys())
    basis_values = list(data['EducationBasisDistribution'].values())
    basis_display = [basis_labels.get(k, k) for k in basis_keys]
    
    if len(basis_keys) == 2:
        colors3 = ['#4CAF50', '#FF9800']
    else:
        colors3 = plt.cm.Set3(np.linspace(0, 1, len(basis_keys)))
    
    if len(basis_values) > 0:
        wedges, texts, autotexts = ax3.pie(basis_values, labels=basis_display, autopct='%1.1f%%',
                                           colors=colors3, startangle=90, shadow=True)
        ax3.set_title('Распределение форм обучения')
        
        for autotext in autotexts:
            autotext.set_color('white')
            autotext.set_fontweight('bold')
    else:
        ax3.text(0.5, 0.5, 'Нет данных', ha='center', va='center', fontsize=12)
        ax3.set_title('Распределение форм обучения')
    
    ax4 = axes[1, 1]
    years = sorted(list(data['EnrollmentByYear'].keys()))
    enrollments = [data['EnrollmentByYear'][year] for year in years]
    
    if len(years) > 0:
        ax4.plot(years, enrollments, marker='o', linestyle='-', linewidth=2, markersize=8,
                 color='#2196F3', markerfacecolor='white', markeredgecolor='#2196F3', markeredgewidth=2)
        ax4.set_xlabel('Год')
        ax4.set_ylabel('Количество поступивших')
        ax4.set_title('Динамика поступления студентов')
        ax4.grid(True, alpha=0.3)
        
        for year, count in zip(years, enrollments):
            offset = max(enrollments) * 0.02 if len(enrollments) > 0 else 1
            ax4.text(year, count + offset, str(count),
                    ha='center', va='bottom', fontsize=9)
    else:
        ax4.text(0.5, 0.5, 'Нет данных', ha='center', va='center', fontsize=12)
        ax4.set_title('Динамика поступления студентов')
    
    plt.tight_layout(rect=[0, 0.03, 1, 0.95])
    
    plt.savefig(output_png, dpi=300, bbox_inches='tight')
    
    with PdfPages(output_pdf) as pdf:
        pdf.savefig(fig, bbox_inches='tight')
    
    plt.close(fig)

if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("Usage: python charts_generator.py <output_png> <output_pdf>", file=sys.stderr)
        sys.exit(1)
    
    output_png = sys.argv[1]
    output_pdf = sys.argv[2]
    
    try:
        data = read_input_data()
        create_charts(data, output_png, output_pdf)
        print(f"Charts generated successfully: {output_png}, {output_pdf}")
        sys.exit(0)
    except Exception as e:
        print(f"Error generating charts: {str(e)}", file=sys.stderr)
        sys.exit(1)