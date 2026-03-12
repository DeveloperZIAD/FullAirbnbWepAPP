# --- المرحلة الأولى: البناء (Build Stage) ---
FROM node:18-alpine AS build
WORKDIR /app





ARG VITE_API_URL
ENV VITE_API_URL=$VITE_API_URL


# نسخ ملفات الـ package و تثبيت المكتبات (لتحسين الـ Cache)
COPY package*.json ./
RUN npm install

# نسخ باقي الكود وعمل Build
COPY . .
RUN npm run build

# --- المرحلة الثانية: التشغيل (Production Stage - Nginx) ---
FROM nginx:alpine
# نسخ الملفات التي تم عمل build لها من المرحلة الأولى إلى سيرفر Nginx
COPY --from=build /app/dist /usr/share/nginx/html

# إضافة إعدادات Nginx (اختياري، لتوجيه كل الطلبات لملف index.html - مهم للـ Routing)
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]