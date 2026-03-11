# المرحلة الأولى: الأساس (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# المرحلة الثانية: البناء (Build)
# المرحلة الثانية: البناء
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# استخدام علامة * للبحث عن ملفات الـ csproj في المجلدات الفرعية
COPY ["Business Logic Layer/*.csproj", "Business Logic Layer/"]
COPY ["Data Access Layer/*.csproj", "Data Access Layer/"]
COPY ["DataGenerator/*.csproj", "DataGenerator/"]
COPY ["Presentation Layer (Web API)/*.csproj", "Presentation Layer (Web API)/"]

# استعادة الحزم (هنا نستخدم المسار الصحيح)
RUN dotnet restore "Presentation Layer (Web API)/Presentation Layer (Web API).csproj"

# نسخ باقي الكود
COPY . .

# البناء
WORKDIR "/src/Presentation Layer (Web API)"
RUN dotnet build "Presentation Layer (Web API).csproj" -c Release -o /app/build

# المرحلة الثالثة: النشر
FROM build AS publish
RUN dotnet publish "Presentation Layer (Web API).csproj" -c Release -o /app/publish

# المرحلة الرابعة: النهائي (النسخة الخفيفة للتشغيل)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# تشغيل التطبيق (استخدم علامات الاقتباس لتجنب مشاكل المسافات)
ENTRYPOINT ["dotnet", "Presentation Layer (Web API).dll"]