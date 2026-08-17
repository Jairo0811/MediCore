from pathlib import Path
import re

path = Path('README.md')
text = path.read_text(encoding='utf-8')
section = '''## 🧭 Continuidad académica

**MediCore** documenta una continuidad académica verificable en dos ejes dentro de la trayectoria de Francis Jairo Matías Rosario en la Universidad APEC (UNAPEC): **estudiante recurrente** y **profesor recurrente**. Estas relaciones son formativas y cronológicas; no implican dependencias técnicas entre las aplicaciones.

### 👥 Continuidad por estudiante

**Emely Marie Castillo Rivera (A00110380)** coincidió con Francis Jairo Matías Rosario en dos proyectos académicos consecutivos de 2026. La primera coincidencia ocurrió en **MediCore**, correspondiente a **Desarrollo de Software con Tecnología Propietaria 1 (ISO-605)**; posteriormente ambos volvieron a integrar el mismo equipo en [**EcoSoft**](https://github.com/Jairo0811/Ecosoft), desarrollado para **Proyecto de Software 1 (ISO-705)**.

| Orden | Asignatura | Proyecto | Período | Estudiante recurrente |
|---:|---|---|---|---|
| 1 | Desarrollo de Software con Tecnología Propietaria 1 (ISO-605) | **MediCore** | Enero - Abril 2026 | **Emely Marie Castillo Rivera — A00110380** |
| 2 | Proyecto de Software 1 (ISO-705) | [**EcoSoft**](https://github.com/Jairo0811/Ecosoft) | Mayo - Agosto 2026 | **Emely Marie Castillo Rivera — A00110380** |

La coincidencia se considera verificada por el **mismo nombre completo y la misma matrícula A00110380**.

### 👨‍🏫 Continuidad por profesor

El profesor **Ing. Omar Antonio De Jesus De La Cruz Gonzalez** impartió durante **Enero - Abril de 2026** dos asignaturas en las que Francis Jairo Matías Rosario participó en proyectos finales distintos: **ISO-605** con MediCore e **ISO-610** con [**CineGest**](https://github.com/Jairo0811/CineGest).

| Orden | Asignatura | Proyecto | Período | Profesor recurrente |
|---:|---|---|---|---|
| 1 | Desarrollo de Software con Tecnología Propietaria 1 (ISO-605) | **MediCore** | Enero - Abril 2026 | **Ing. Omar Antonio De Jesus De La Cruz Gonzalez** |
| 2 | Desarrollo de Software con Tecnología Open Source 1 (ISO-610) | [**CineGest**](https://github.com/Jairo0811/CineGest) | Enero - Abril 2026 | **Ing. Omar Antonio De Jesus De La Cruz Gonzalez** |

Esta continuidad docente documenta una experiencia paralela en dos líneas complementarias del plan de estudios: **tecnología propietaria** y **tecnología open source**.

### 🏫 Cruce institucional ITLA → UNAPEC

MediCore también documenta un cruce institucional entre **ITLA** y **UNAPEC**. Además de Francis Jairo Matías Rosario, **José Samuel Peña Acevedo**, integrante del equipo académico original de MediCore, cursó estudios previamente en el **Instituto Tecnológico de Las Américas (ITLA)** antes de coincidir posteriormente en este proyecto de UNAPEC.

Este vínculo se documenta como una **trayectoria institucional compartida**, no como continuidad por asignaturas cursadas conjuntamente en ITLA. Con la información disponible no se establece que Francis Jairo Matías Rosario y José Samuel Peña Acevedo hayan coincidido en una misma materia durante su etapa en esa institución; la coincidencia académica confirmada corresponde a **MediCore (ISO-605, Enero - Abril 2026)** en UNAPEC.

| Integrante | Matrícula UNAPEC | Matrícula ITLA | Relación documentada |
|---|---|---|---|
| **Francis Jairo Matías Rosario** | A00115261 | **2015-2984** | ITLA → UNAPEC |
| **José Samuel Peña Acevedo** | A00107391 | **2017-4611** | ITLA → UNAPEC; coincidencia posterior en MediCore |

Este cruce es adicional a la continuidad por estudiante y profesor y no se utiliza como evidencia de materias compartidas en ITLA.
'''
pattern = r'## 🧭 Continuidad académica.*?(?=\n## 🧩 Dominios)'
new = re.sub(pattern, section.rstrip(), text, flags=re.S)
if new == text:
    raise SystemExit('Continuity section not found')
path.write_text(new, encoding='utf-8')
