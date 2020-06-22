import org.jetbrains.kotlin.gradle.tasks.KotlinCompile

plugins {
    id("org.springframework.boot") version "2.3.0.RELEASE"
    id("io.spring.dependency-management") version "1.0.9.RELEASE"
    war
    kotlin("jvm") version "1.3.72"
    kotlin("plugin.spring") version "1.3.72"
    kotlin("plugin.jpa") version "1.3.72"
    kotlin("plugin.allopen") version "1.3.61"
    id("com.novoda.build-properties") version "0.4.1"
}

group = "khitiara.ffxiv"
version = "0.0.1-SNAPSHOT"
java.sourceCompatibility = JavaVersion.VERSION_1_8

repositories {
    mavenCentral()
    jcenter()
}

dependencies {
    implementation("org.springframework.boot:spring-boot-starter-data-jpa")
    implementation("org.springframework.boot:spring-boot-starter-thymeleaf")
    implementation("org.springframework.boot:spring-boot-starter-webflux")
    implementation("org.springframework.boot:spring-boot-starter-actuator")
    implementation("org.springframework.boot:spring-boot-starter-cache")
    implementation("com.fasterxml.jackson.module:jackson-module-kotlin")
    developmentOnly("org.springframework.boot:spring-boot-devtools")

    implementation("net.dv8tion:JDA:4.1.1_156") {
        exclude(module = "opus-java")
    }
    implementation("club.minnced:jda-reactor:1.1.0")

    implementation("org.jetbrains.kotlin:kotlin-reflect")
    implementation("org.jetbrains.kotlin:kotlin-stdlib-jdk8")
    implementation("org.jetbrains.kotlinx:kotlinx-coroutines-core:1.3.7")
    implementation("org.jetbrains.kotlinx:kotlinx-coroutines-reactor:1.3.7")
    runtimeOnly("org.postgresql:postgresql")
    providedRuntime("org.springframework.boot:spring-boot-starter-tomcat")
    implementation("com.vladmihalcea:hibernate-types-52:2.9.11")

    implementation("org.webjars:webjars-locator:0.40")
    implementation("org.webjars:jquery:3.5.1")
    implementation("org.webjars:bootstrap:4.5.0")
}

buildProperties {
    create("secrets") {
        using(project.file("secrets.properties"))
    }
}

tasks.withType<Test> {
    useJUnitPlatform()
}

tasks.withType<KotlinCompile> {
    kotlinOptions {
        freeCompilerArgs = listOf("-Xjsr305=strict")
        jvmTarget = "1.8"
    }
}

allOpen {
    annotation("javax.persistence.Entity")
    annotation("javax.persistence.Embeddable")
    annotation("javax.persistence.MappedSuperclass")
}

tasks.processResources {
    from("${project.rootDir}/classjob-icons/icons") {
        into("resources/cj-icons")
    }
    from(sourceSets["main"].resources.srcDirs) {
        include("**/*.properties")
        expand(buildProperties["secrets"].asMap().mapValues { it.value.string })
    }
    from(sourceSets["main"].resources.srcDirs) {
        exclude("**/*.properties")
    }
}