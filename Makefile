# أوامر المشروع المختصرة
.PHONY: up down build logs clean test up-logs

# 1. التشغيل العادي (في الخلفية)
up:
	docker-compose up --build -d

# 2. التشغيل مع مراقبة الـ Logs (لمعرفة الأخطاء فوراً)
up-logs:
	docker-compose up --build

# 3. إيقاف المشروع بالكامل
down:
	docker-compose down

# 4. بناء الصور فقط (للتأكد من عدم وجود أخطاء في الـ Dockerfile)
build:
	docker-compose build

# 5. عرض الـ Logs للـ backend فقط
logs:
	docker-compose logs -f backend

# 6. تنظيف شامل (حذف الصور والحاويات والبيانات)
clean:
	docker-compose down -v --rmi all