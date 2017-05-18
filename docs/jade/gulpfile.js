var gulp = require('gulp'),
    watch = require('gulp-watch'),
    pug = require('gulp-pug');

gulp.task('build-pug', function () {
    return gulp.src(['src/index.pug', 'src/BH*.pug', 'src/404.pug'])
        .pipe(pug()) // pipe to pug plugin
        .pipe(gulp.dest('../')); // tell gulp our output folder
});

gulp.task('watch', function () {
    gulp.watch('src/**/*.pug', ['build-pug']);
});
