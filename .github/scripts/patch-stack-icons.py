from pathlib import Path

p = Path('README.md')
s = p.read_text(encoding='utf-8')

s = s.replace('<p align="center"><img src="https://skillicons.dev/icons?i=cs,dotnet,react,ts,vite,docker,git,github,githubactions" alt="C#, .NET, React, TypeScript, Vite, Docker, Git, GitHub y GitHub Actions" /></p>\n\n', '')
s = s.replace('### Backend\n\n', '### ⚙️ Backend\n\n<p>\n  <img src="https://skillicons.dev/icons?i=cs,dotnet" alt="C# y .NET" />\n</p>\n\n')
s = s.replace('### Frontend\n\n', '### 🎨 Frontend\n\n<p>\n  <img src="https://skillicons.dev/icons?i=react,ts,vite,html,css" alt="React, TypeScript, Vite, HTML y CSS" />\n</p>\n\n')
old = '''### Datos e infraestructura

- **Microsoft SQL Server 2022**
- **EF Core Migrations**
- **Docker / Docker Compose**
- **Git / GitHub**
- **GitHub Actions**
- **xUnit**
'''
new = '''### 🗄️ Datos

<p>
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/microsoftsqlserver/microsoftsqlserver-plain.svg" alt="Microsoft SQL Server" width="52" height="52" />
</p>

- **Microsoft SQL Server 2022**
- **EF Core Migrations**

### 🧰 Infraestructura y calidad

<p>
  <img src="https://skillicons.dev/icons?i=docker,git,github,githubactions" alt="Docker, Git, GitHub y GitHub Actions" />
</p>

- **Docker / Docker Compose**
- **Git / GitHub**
- **GitHub Actions**
- **xUnit**
'''
if old not in s:
    raise SystemExit('MediCore stack block not found')
s = s.replace(old, new, 1)
p.write_text(s, encoding='utf-8')
