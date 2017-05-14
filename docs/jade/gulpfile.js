var gulp = require('gulp'),
    watch = require('gulp-watch'),
    pug = require('gulp-pug');

gulp.task('build-pug', function () {
    return gulp.src(['src/**/*.pug'])
        .pipe(pug()) // pipe to pug plugin
        .pipe(gulp.dest('build')); // tell gulp our output folder
});

gulp.task('watch', function () {
    gulp.watch('src/**/*.pug', ['build-pug']);
});